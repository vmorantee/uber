using Microsoft.AspNetCore.SignalR;

namespace RideSharingAPI.Hubs;

public class RideHub : Hub
{
    private readonly ILogger<RideHub> _logger;

    public RideHub(ILogger<RideHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinRideGroup(int rideId)
    {
        var groupName = $"Ride_{rideId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation($"Client {Context.ConnectionId} joined group {groupName}");
    }

    public async Task LeaveRideGroup(int rideId)
    {
        var groupName = $"Ride_{rideId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation($"Client {Context.ConnectionId} left group {groupName}");
    }

    public async Task JoinDriverChannel(int driverId)
    {
        var groupName = $"Driver_{driverId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation($"Client {Context.ConnectionId} joined driver channel {groupName}");
    }

    public async Task UpdateDriverLocation(int driverId, decimal latitude, decimal longitude)
    {
        await Clients.Group($"Driver_{driverId}").SendAsync("DriverLocationUpdated", new
        {
            DriverId = driverId,
            Latitude = latitude,
            Longitude = longitude,
            Timestamp = DateTime.UtcNow
        });
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }
}

public interface IRideHubClient
{
    Task RideStatusChanged(object rideUpdate);
    Task DriverLocationUpdated(object locationUpdate);
    Task NewRideRequest(object rideRequest);
    Task RideAccepted(object rideInfo);
    Task RideCompleted(object rideInfo);
}
