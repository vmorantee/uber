using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RideSharingAPI.Hubs;
using RideSharingAPI.Models;
using RideSharingAPI.Repositories;
using RideSharingAPI.Services;

namespace RideSharingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RideController : ControllerBase
{
    private readonly IRideRepository _rideRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IDriverLocationRepository _driverLocationRepository;
    private readonly IHubContext<RideHub> _hubContext;
    private readonly IServiceBusSender _serviceBusSender;
    private readonly ILogger<RideController> _logger;

    public RideController(
        IRideRepository rideRepository,
        IWalletRepository walletRepository,
        IDriverLocationRepository driverLocationRepository,
        IHubContext<RideHub> hubContext,
        IServiceBusSender serviceBusSender,
        ILogger<RideController> logger)
    {
        _rideRepository = rideRepository;
        _walletRepository = walletRepository;
        _driverLocationRepository = driverLocationRepository;
        _hubContext = hubContext;
        _serviceBusSender = serviceBusSender;
        _logger = logger;
    }

    [HttpPost("estimate")]
    public ActionResult<EstimatePriceResponse> EstimatePrice([FromBody] CreateRideRequest request)
    {
        var distance = CalculateDistance(
            request.StartLocationLat,
            request.StartLocationLng,
            request.EndLocationLat,
            request.EndLocationLng);

        var basePrice = 5.0m;
        var pricePerKm = 2.5m;
        var estimatedPrice = basePrice + (distance * pricePerKm);

        return Ok(new EstimatePriceResponse
        {
            EstimatedPrice = Math.Round(estimatedPrice, 2),
            Distance = Math.Round(distance, 2)
        });
    }

    [HttpPost]
    public async Task<ActionResult<RideDto>> CreateRide([FromBody] CreateRideRequest request, [FromQuery] int passengerId)
    {
        var distance = CalculateDistance(
            request.StartLocationLat,
            request.StartLocationLng,
            request.EndLocationLat,
            request.EndLocationLng);

        var basePrice = 5.0m;
        var pricePerKm = 2.5m;
        var price = basePrice + (distance * pricePerKm);

        var ride = new Ride
        {
            PassengerID = passengerId,
            StartLocationLat = request.StartLocationLat,
            StartLocationLng = request.StartLocationLng,
            EndLocationLat = request.EndLocationLat,
            EndLocationLng = request.EndLocationLng,
            StartAddress = request.StartAddress,
            EndAddress = request.EndAddress,
            Price = Math.Round(price, 2),
            Distance = Math.Round(distance, 2),
            Status = "SEARCHING",
            StartTime = DateTime.UtcNow
        };

        var createdRide = await _rideRepository.CreateAsync(ride);

        await _hubContext.Clients.All.SendAsync("NewRideRequest", new
        {
            RideId = createdRide.ID,
            PassengerId = createdRide.PassengerID,
            StartLat = createdRide.StartLocationLat,
            StartLng = createdRide.StartLocationLng,
            EndLat = createdRide.EndLocationLat,
            EndLng = createdRide.EndLocationLng,
            Price = createdRide.Price,
            Distance = createdRide.Distance
        });

        return Ok(MapToDto(createdRide));
    }

    [HttpPost("{rideId}/accept")]
    public async Task<ActionResult<RideDto>> AcceptRide(int rideId, [FromQuery] int driverId)
    {
        var ride = await _rideRepository.GetByIdAsync(rideId);
        
        if (ride == null)
        {
            return NotFound("Ride not found");
        }

        if (ride.Status != "SEARCHING")
        {
            return BadRequest("Ride is not available");
        }

        ride.DriverID = driverId;
        ride.Status = "ACCEPTED";
        ride.AcceptedTime = DateTime.UtcNow;

        var updatedRide = await _rideRepository.UpdateAsync(ride);

        await _hubContext.Clients.Group($"Ride_{rideId}").SendAsync("RideAccepted", new
        {
            RideId = updatedRide.ID,
            DriverId = driverId,
            Status = "ACCEPTED",
            AcceptedTime = updatedRide.AcceptedTime
        });

        return Ok(MapToDto(updatedRide));
    }

    [HttpPost("{rideId}/start")]
    public async Task<ActionResult<RideDto>> StartRide(int rideId)
    {
        var ride = await _rideRepository.GetByIdAsync(rideId);
        
        if (ride == null)
        {
            return NotFound("Ride not found");
        }

        if (ride.Status != "ACCEPTED")
        {
            return BadRequest("Ride must be accepted before starting");
        }

        ride.Status = "IN_PROGRESS";
        var updatedRide = await _rideRepository.UpdateAsync(ride);

        await _hubContext.Clients.Group($"Ride_{rideId}").SendAsync("RideStatusChanged", new
        {
            RideId = updatedRide.ID,
            Status = "IN_PROGRESS"
        });

        return Ok(MapToDto(updatedRide));
    }

    [HttpPost("{rideId}/complete")]
    public async Task<ActionResult<RideDto>> CompleteRide(int rideId)
    {
        var ride = await _rideRepository.GetByIdAsync(rideId);
        
        if (ride == null)
        {
            return NotFound("Ride not found");
        }

        if (ride.Status != "IN_PROGRESS")
        {
            return BadRequest("Ride must be in progress to complete");
        }

        if (!ride.DriverID.HasValue)
        {
            return BadRequest("No driver assigned");
        }

        var paymentSuccess = await _walletRepository.ProcessRidePaymentAsync(
            ride.PassengerID,
            ride.DriverID.Value,
            ride.Price,
            ride.ID);

        if (!paymentSuccess)
        {
            return BadRequest("Payment failed - insufficient balance");
        }

        ride.Status = "COMPLETED";
        ride.CompletedTime = DateTime.UtcNow;
        var updatedRide = await _rideRepository.UpdateAsync(ride);

        await _serviceBusSender.SendRideCompletedMessageAsync(
            ride.ID,
            ride.PassengerID,
            ride.DriverID.Value,
            ride.Price);

        await _hubContext.Clients.Group($"Ride_{rideId}").SendAsync("RideCompleted", new
        {
            RideId = updatedRide.ID,
            Status = "COMPLETED",
            CompletedTime = updatedRide.CompletedTime
        });

        return Ok(MapToDto(updatedRide));
    }

    [HttpPost("{rideId}/cancel")]
    public async Task<ActionResult<RideDto>> CancelRide(int rideId)
    {
        var ride = await _rideRepository.GetByIdAsync(rideId);
        
        if (ride == null)
        {
            return NotFound("Ride not found");
        }

        if (ride.Status == "COMPLETED")
        {
            return BadRequest("Cannot cancel completed ride");
        }

        ride.Status = "CANCELLED";
        ride.CancelledTime = DateTime.UtcNow;
        var updatedRide = await _rideRepository.UpdateAsync(ride);

        await _hubContext.Clients.Group($"Ride_{rideId}").SendAsync("RideStatusChanged", new
        {
            RideId = updatedRide.ID,
            Status = "CANCELLED"
        });

        return Ok(MapToDto(updatedRide));
    }

    [HttpGet("{rideId}")]
    public async Task<ActionResult<RideDto>> GetRide(int rideId)
    {
        var ride = await _rideRepository.GetByIdAsync(rideId);
        
        if (ride == null)
        {
            return NotFound("Ride not found");
        }

        return Ok(MapToDto(ride));
    }

    [HttpGet("passenger/{passengerId}")]
    public async Task<ActionResult<List<RideDto>>> GetPassengerRides(int passengerId)
    {
        var rides = await _rideRepository.GetRidesByPassengerAsync(passengerId);
        return Ok(rides.Select(MapToDto).ToList());
    }

    [HttpGet("driver/{driverId}")]
    public async Task<ActionResult<List<RideDto>>> GetDriverRides(int driverId)
    {
        var rides = await _rideRepository.GetRidesByDriverAsync(driverId);
        return Ok(rides.Select(MapToDto).ToList());
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<RideDto>>> GetActiveRides()
    {
        var rides = await _rideRepository.GetActiveRidesAsync();
        return Ok(rides.Select(MapToDto).ToList());
    }

    [HttpPost("location/update")]
    public async Task<IActionResult> UpdateDriverLocation([FromBody] UpdateLocationRequest request, [FromQuery] int driverId)
    {
        await _driverLocationRepository.UpdateLocationAsync(
            driverId,
            request.Latitude,
            request.Longitude,
            request.IsOnline);

        await _hubContext.Clients.All.SendAsync("DriverLocationUpdated", new
        {
            DriverId = driverId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            IsOnline = request.IsOnline,
            Timestamp = DateTime.UtcNow
        });

        return Ok();
    }

    private decimal CalculateDistance(decimal lat1, decimal lng1, decimal lat2, decimal lng2)
    {
        var R = 6371m;
        var dLat = ToRadians((double)(lat2 - lat1));
        var dLng = ToRadians((double)(lng2 - lng1));
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * (decimal)c;
    }

    private double ToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }

    private RideDto MapToDto(Ride ride)
    {
        return new RideDto
        {
            ID = ride.ID,
            PassengerID = ride.PassengerID,
            DriverID = ride.DriverID,
            StartLocationLat = ride.StartLocationLat,
            StartLocationLng = ride.StartLocationLng,
            EndLocationLat = ride.EndLocationLat,
            EndLocationLng = ride.EndLocationLng,
            StartAddress = ride.StartAddress,
            EndAddress = ride.EndAddress,
            Price = ride.Price,
            Distance = ride.Distance,
            Status = ride.Status,
            StartTime = ride.StartTime,
            AcceptedTime = ride.AcceptedTime,
            CompletedTime = ride.CompletedTime,
            PassengerName = ride.Passenger != null ? $"{ride.Passenger.FirstName} {ride.Passenger.LastName}" : null,
            DriverName = ride.Driver != null ? $"{ride.Driver.FirstName} {ride.Driver.LastName}" : null
        };
    }
}
