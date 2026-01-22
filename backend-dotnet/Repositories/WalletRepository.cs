using Microsoft.EntityFrameworkCore;
using RideSharingAPI.Data;
using RideSharingAPI.Models;

namespace RideSharingAPI.Repositories;

public interface IWalletRepository
{
    Task<Wallet?> GetByUserIdAsync(int userId);
    Task<Wallet> CreateAsync(Wallet wallet);
    Task<Wallet> UpdateAsync(Wallet wallet);
    Task<WalletTransaction> AddTransactionAsync(WalletTransaction transaction);
    Task<List<WalletTransaction>> GetTransactionsByWalletIdAsync(int walletId);
    Task<bool> ProcessRidePaymentAsync(int passengerId, int driverId, decimal amount, int rideId);
}

public class WalletRepository : IWalletRepository
{
    private readonly RideSharingContext _context;
    private const decimal PLATFORM_COMMISSION = 0.20m;

    public WalletRepository(RideSharingContext context)
    {
        _context = context;
    }

    public async Task<Wallet?> GetByUserIdAsync(int userId)
    {
        return await _context.Wallets.FirstOrDefaultAsync(w => w.UserID == userId);
    }

    public async Task<Wallet> CreateAsync(Wallet wallet)
    {
        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }

    public async Task<Wallet> UpdateAsync(Wallet wallet)
    {
        wallet.UpdatedAt = DateTime.UtcNow;
        _context.Wallets.Update(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }

    public async Task<WalletTransaction> AddTransactionAsync(WalletTransaction transaction)
    {
        _context.WalletTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<List<WalletTransaction>> GetTransactionsByWalletIdAsync(int walletId)
    {
        return await _context.WalletTransactions
            .Where(wt => wt.WalletID == walletId)
            .OrderByDescending(wt => wt.TransactionDate)
            .ToListAsync();
    }

    public async Task<bool> ProcessRidePaymentAsync(int passengerId, int driverId, decimal amount, int rideId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var passengerWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserID == passengerId);
            var driverWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserID == driverId);

            if (passengerWallet == null || driverWallet == null)
            {
                return false;
            }

            if (passengerWallet.Balance < amount)
            {
                return false;
            }

            passengerWallet.Balance -= amount;
            passengerWallet.UpdatedAt = DateTime.UtcNow;

            var driverEarning = amount * (1 - PLATFORM_COMMISSION);
            driverWallet.Balance += driverEarning;
            driverWallet.UpdatedAt = DateTime.UtcNow;

            var passengerTransaction = new WalletTransaction
            {
                WalletID = passengerWallet.ID,
                Amount = -amount,
                Type = "RidePayment",
                TransactionDate = DateTime.UtcNow,
                ReferenceID = rideId,
                Description = $"Payment for ride #{rideId}"
            };

            var driverTransaction = new WalletTransaction
            {
                WalletID = driverWallet.ID,
                Amount = driverEarning,
                Type = "Earning",
                TransactionDate = DateTime.UtcNow,
                ReferenceID = rideId,
                Description = $"Earning from ride #{rideId}"
            };

            _context.Wallets.Update(passengerWallet);
            _context.Wallets.Update(driverWallet);
            _context.WalletTransactions.Add(passengerTransaction);
            _context.WalletTransactions.Add(driverTransaction);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }
}
