using Microsoft.EntityFrameworkCore;
using RideSharingAPI.Data;
using RideSharingAPI.Models;

namespace RideSharingAPI.Repositories;

public interface IRideRepository
{
    Task<Ride> CreateAsync(Ride ride);
    Task<Ride?> GetByIdAsync(int id);
    Task<List<Ride>> GetActiveRidesAsync();
    Task<List<Ride>> GetRidesByPassengerAsync(int passengerId);
    Task<List<Ride>> GetRidesByDriverAsync(int driverId);
    Task<Ride> UpdateAsync(Ride ride);
    Task<Ride?> GetSearchingRideAsync();
}

public class RideRepository : IRideRepository
{
    private readonly RideSharingContext _context;

    public RideRepository(RideSharingContext context)
    {
        _context = context;
    }

    public async Task<Ride> CreateAsync(Ride ride)
    {
        _context.Rides.Add(ride);
        await _context.SaveChangesAsync();
        return ride;
    }

    public async Task<Ride?> GetByIdAsync(int id)
    {
        return await _context.Rides
            .Include(r => r.Passenger)
            .Include(r => r.Driver)
            .FirstOrDefaultAsync(r => r.ID == id);
    }

    public async Task<List<Ride>> GetActiveRidesAsync()
    {
        return await _context.Rides
            .Include(r => r.Passenger)
            .Include(r => r.Driver)
            .Where(r => r.Status == "SEARCHING" || r.Status == "ACCEPTED" || r.Status == "IN_PROGRESS")
            .ToListAsync();
    }

    public async Task<List<Ride>> GetRidesByPassengerAsync(int passengerId)
    {
        return await _context.Rides
            .Include(r => r.Driver)
            .Where(r => r.PassengerID == passengerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Ride>> GetRidesByDriverAsync(int driverId)
    {
        return await _context.Rides
            .Include(r => r.Passenger)
            .Where(r => r.DriverID == driverId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Ride> UpdateAsync(Ride ride)
    {
        _context.Rides.Update(ride);
        await _context.SaveChangesAsync();
        return ride;
    }

    public async Task<Ride?> GetSearchingRideAsync()
    {
        return await _context.Rides
            .Include(r => r.Passenger)
            .FirstOrDefaultAsync(r => r.Status == "SEARCHING");
    }
}
