namespace RideSharingAPI.Models;

public class User
{
    public int ID { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = "Passenger";
    public bool IsDriverApproved { get; set; } = false;
    public string CurrentContext { get; set; } = "Passenger";
    public string? AvatarUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public string? VehicleModel { get; set; }
    public string? LicensePlate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public Wallet? Wallet { get; set; }
}

public class Wallet
{
    public int ID { get; set; }
    public int UserID { get; set; }
    public decimal Balance { get; set; } = 0;
    public string Currency { get; set; } = "USD";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public User? User { get; set; }
}

public class WalletTransaction
{
    public int ID { get; set; }
    public int WalletID { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public int? ReferenceID { get; set; }
    public string? Description { get; set; }
}

public class Ride
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
    public string Status { get; set; } = "SEARCHING";
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? AcceptedTime { get; set; }
    public DateTime? CompletedTime { get; set; }
    public DateTime? CancelledTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public User? Passenger { get; set; }
    public User? Driver { get; set; }
}

public class DriverLocation
{
    public int ID { get; set; }
    public int DriverID { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsOnline { get; set; } = false;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class Dispute
{
    public int ID { get; set; }
    public int RideID { get; set; }
    public int ReportingUserID { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "OPEN";
    public string? ResolutionDecision { get; set; }
    public decimal RefundAmount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
    public int? ResolvedByAdminID { get; set; }
}

public class Banner
{
    public int ID { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? TargetUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
