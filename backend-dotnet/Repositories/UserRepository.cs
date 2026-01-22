using Microsoft.EntityFrameworkCore;
using RideSharingAPI.Data;
using RideSharingAPI.Models;

namespace RideSharingAPI.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<List<User>> GetAllAsync();
}

public class UserRepository : IUserRepository
{
    private readonly RideSharingContext _context;

    public UserRepository(RideSharingContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Wallet)
            .FirstOrDefaultAsync(u => u.ID == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Wallet)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Wallet)
            .ToListAsync();
    }
}

public interface IBannerRepository
{
    Task<List<Banner>> GetActiveBannersAsync();
}

public class BannerRepository : IBannerRepository
{
    private readonly RideSharingContext _context;

    public BannerRepository(RideSharingContext context)
    {
        _context = context;
    }

    public async Task<List<Banner>> GetActiveBannersAsync()
    {
        return await _context.Banners
            .Where(b => b.IsActive)
            .OrderBy(b => b.DisplayOrder)
            .ToListAsync();
    }
}

public interface IDriverLocationRepository
{
    Task UpdateLocationAsync(int driverId, decimal latitude, decimal longitude, bool isOnline);
    Task<DriverLocation?> GetByDriverIdAsync(int driverId);
}

public class DriverLocationRepository : IDriverLocationRepository
{
    private readonly RideSharingContext _context;

    public DriverLocationRepository(RideSharingContext context)
    {
        _context = context;
    }

    public async Task UpdateLocationAsync(int driverId, decimal latitude, decimal longitude, bool isOnline)
    {
        var location = await _context.DriverLocations.FirstOrDefaultAsync(dl => dl.DriverID == driverId);
        
        if (location == null)
        {
            location = new DriverLocation
            {
                DriverID = driverId,
                Latitude = latitude,
                Longitude = longitude,
                IsOnline = isOnline,
                UpdatedAt = DateTime.UtcNow
            };
            _context.DriverLocations.Add(location);
        }
        else
        {
            location.Latitude = latitude;
            location.Longitude = longitude;
            location.IsOnline = isOnline;
            location.UpdatedAt = DateTime.UtcNow;
            _context.DriverLocations.Update(location);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<DriverLocation?> GetByDriverIdAsync(int driverId)
    {
        return await _context.DriverLocations.FirstOrDefaultAsync(dl => dl.DriverID == driverId);
    }
}
