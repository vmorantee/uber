using LandingPage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace LandingPage.Controllers;

public class HomeController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;

    public HomeController(IConfiguration configuration, ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult BecomeDriver()
    {
        return View(new DriverApplicationViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> BecomeDriver(DriverApplicationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var apiBaseUrl = _configuration["ApiBaseUrl"] ?? "http://localhost:5000";
            
            var registerRequest = new
            {
                Email = model.Email,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Role = "Driver",
                VehicleModel = model.VehicleModel,
                LicensePlate = model.LicensePlate
            };

            var json = JsonSerializer.Serialize(registerRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{apiBaseUrl}/api/auth/register", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Your driver application has been submitted successfully! Please wait for admin approval.";
                return RedirectToAction("Success");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"API Error: {errorContent}");
                ModelState.AddModelError(string.Empty, "Failed to submit application. Please try again.");
                return View(model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting driver application");
            ModelState.AddModelError(string.Empty, "An error occurred. Please try again later.");
            return View(model);
        }
    }

    public IActionResult Success()
    {
        return View();
    }
}
