namespace RideSharingAPI.Models;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = "Passenger";
    public string? VehicleModel { get; set; }
    public string? LicensePlate { get; set; }
}

public class CreateRideRequest
{
    public decimal StartLocationLat { get; set; }
    public decimal StartLocationLng { get; set; }
    public decimal EndLocationLat { get; set; }
    public decimal EndLocationLng { get; set; }
    public string? StartAddress { get; set; }
    public string? EndAddress { get; set; }
}

public class TopUpRequest
{
    public decimal Amount { get; set; }
}

public class UpdateLocationRequest
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsOnline { get; set; }
}

public class EstimatePriceResponse
{
    public decimal EstimatedPrice { get; set; }
    public decimal Distance { get; set; }
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = new();
}

public class UserDto
{
    public int ID { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsDriverApproved { get; set; }
    public string CurrentContext { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public decimal WalletBalance { get; set; }
}

public class RideDto
{
    public int ID { get; set; }
    public int PassengerID { get; set; }
    public int? DriverID { get; set; }
    public decimal StartLocationLat { get; set; }
    public decimal StartLocationLng { get; set; }
    public decimal EndLocationLat { get; set; }
    public decimal EndLocationLng { get; set; }
    public string? StartAddress { get; set; }
    public string? EndAddress { get; set; }
    public decimal Price { get; set; }
    public decimal? Distance { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? AcceptedTime { get; set; }
    public DateTime? CompletedTime { get; set; }
    public string? PassengerName { get; set; }
    public string? DriverName { get; set; }
}
