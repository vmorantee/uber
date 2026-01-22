using Microsoft.AspNetCore.Mvc;
using RideSharingAPI.Models;
using RideSharingAPI.Repositories;

namespace RideSharingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<WalletController> _logger;

    public WalletController(
        IWalletRepository walletRepository,
        IUserRepository userRepository,
        ILogger<WalletController> logger)
    {
        _walletRepository = walletRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpPost("topup")]
    public async Task<IActionResult> TopUp([FromBody] TopUpRequest request, [FromQuery] int userId)
    {
        if (request.Amount <= 0)
        {
            return BadRequest("Amount must be greater than zero");
        }

        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        
        if (wallet == null)
        {
            wallet = new Wallet
            {
                UserID = userId,
                Balance = 0,
                Currency = "USD"
            };
            wallet = await _walletRepository.CreateAsync(wallet);
        }

        wallet.Balance += request.Amount;
        await _walletRepository.UpdateAsync(wallet);

        var transaction = new WalletTransaction
        {
            WalletID = wallet.ID,
            Amount = request.Amount,
            Type = "TopUp",
            TransactionDate = DateTime.UtcNow,
            Description = $"Top-up of {request.Amount} {wallet.Currency}"
        };

        await _walletRepository.AddTransactionAsync(transaction);

        _logger.LogInformation($"User {userId} topped up {request.Amount} {wallet.Currency}. New balance: {wallet.Balance}");

        return Ok(new
        {
            TransactionId = transaction.ID,
            NewBalance = wallet.Balance,
            Amount = request.Amount,
            Currency = wallet.Currency,
            Timestamp = transaction.TransactionDate
        });
    }

    [HttpGet("balance/{userId}")]
    public async Task<ActionResult<object>> GetBalance(int userId)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        
        if (wallet == null)
        {
            return NotFound("Wallet not found");
        }

        return Ok(new
        {
            UserId = userId,
            Balance = wallet.Balance,
            Currency = wallet.Currency,
            LastUpdated = wallet.UpdatedAt
        });
    }

    [HttpGet("transactions/{userId}")]
    public async Task<ActionResult<List<WalletTransaction>>> GetTransactions(int userId)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        
        if (wallet == null)
        {
            return NotFound("Wallet not found");
        }

        var transactions = await _walletRepository.GetTransactionsByWalletIdAsync(wallet.ID);
        return Ok(transactions);
    }
}
