using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace RideSharingAPI.Services;

public interface IServiceBusSender
{
    Task SendRideCompletedMessageAsync(int rideId, int passengerId, int driverId, decimal amount);
}

public class ServiceBusSender : IServiceBusSender
{
    private readonly ServiceBusClient? _client;
    private readonly ILogger<ServiceBusSender> _logger;
    private readonly bool _isConfigured;

    public ServiceBusSender(IConfiguration configuration, ILogger<ServiceBusSender> logger)
    {
        _logger = logger;
        var connectionString = configuration["AzureServiceBus:ConnectionString"];
        
        if (!string.IsNullOrEmpty(connectionString) && !connectionString.Contains("localhost"))
        {
            try
            {
                _client = new ServiceBusClient(connectionString);
                _isConfigured = true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Azure Service Bus not configured properly, running in simulation mode");
                _isConfigured = false;
            }
        }
        else
        {
            _logger.LogInformation("Azure Service Bus connection string not found, running in simulation mode");
            _isConfigured = false;
        }
    }

    public async Task SendRideCompletedMessageAsync(int rideId, int passengerId, int driverId, decimal amount)
    {
        var message = new
        {
            RideId = rideId,
            PassengerId = passengerId,
            DriverId = driverId,
            Amount = amount,
            CompletedAt = DateTime.UtcNow,
            EventType = "RideCompleted"
        };

        var messageBody = JsonSerializer.Serialize(message);

        if (_isConfigured && _client != null)
        {
            try
            {
                var sender = _client.CreateSender("rides-completed");
                var serviceBusMessage = new ServiceBusMessage(messageBody)
                {
                    ContentType = "application/json",
                    Subject = "RideCompleted"
                };

                await sender.SendMessageAsync(serviceBusMessage);
                _logger.LogInformation($"Sent ride completed message to Service Bus: RideId={rideId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message to Service Bus");
            }
        }
        else
        {
            _logger.LogInformation($"[SIMULATED] Service Bus Message: {messageBody}");
        }
    }
}
