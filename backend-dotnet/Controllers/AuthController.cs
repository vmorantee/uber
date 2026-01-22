using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RideSharingAPI.Models;
using RideSharingAPI.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RideSharingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserRepository userRepository,
        IWalletRepository walletRepository,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _userRepository = userRepository;
        _walletRepository = walletRepository;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest("User with this email already exists");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Role = request.Role,
            IsDriverApproved = request.Role == "Driver" ? false : false,
            CurrentContext = "Passenger",
            VehicleModel = request.VehicleModel,
            LicensePlate = request.LicensePlate
        };

        var createdUser = await _userRepository.CreateAsync(user);

        var wallet = new Wallet
        {
            UserID = createdUser.ID,
            Balance = 0,
            Currency = "USD"
        };
        await _walletRepository.CreateAsync(wallet);

        createdUser.Wallet = wallet;

        var token = GenerateJwtToken(createdUser);

        return Ok(new AuthResponse
        {
            Token = token,
            User = MapToUserDto(createdUser)
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        
        if (user == null)
        {
            return Unauthorized("Invalid credentials");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid credentials");
        }

        var token = GenerateJwtToken(user);

        return Ok(new AuthResponse
        {
            Token = token,
            User = MapToUserDto(user)
        });
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<UserDto>> GetUser(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            return NotFound("User not found");
        }

        return Ok(MapToUserDto(user));
    }

    [HttpPut("user/{userId}/context")]
    public async Task<ActionResult<UserDto>> UpdateContext(int userId, [FromBody] string newContext)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            return NotFound("User not found");
        }

        if (newContext == "Driver" && !user.IsDriverApproved)
        {
            return BadRequest("User is not approved as driver");
        }

        user.CurrentContext = newContext;
        await _userRepository.UpdateAsync(user);

        return Ok(MapToUserDto(user));
    }

    [HttpGet("banners")]
    public async Task<ActionResult<List<Banner>>> GetBanners([FromServices] IBannerRepository bannerRepository)
    {
        var banners = await bannerRepository.GetActiveBannersAsync();
        return Ok(banners);
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "RideSharing_Super_Secret_Key_For_JWT_Token_Generation_2026"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "RideSharingAPI",
            audience: _configuration["Jwt:Audience"] ?? "RideSharingClients",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            ID = user.ID,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            IsDriverApproved = user.IsDriverApproved,
            CurrentContext = user.CurrentContext,
            AvatarUrl = user.AvatarUrl,
            PhoneNumber = user.PhoneNumber,
            WalletBalance = user.Wallet?.Balance ?? 0
        };
    }
}
