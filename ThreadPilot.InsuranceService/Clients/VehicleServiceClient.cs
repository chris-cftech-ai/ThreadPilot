using System.Text.Json;
using ThreadPilot.InsuranceService.Models;
using ThreadPilot.InsuranceService.Responses;

namespace ThreadPilot.InsuranceService.Clients;

public class VehicleServiceClient(
    HttpClient httpClient,
    ILogger<VehicleServiceClient> logger)
    : IVehicleServiceClient
{
    public async Task<VehicleInfo?> GetVehicleAsync(string registrationNumber)
    {
        try
        {
            var url = $"vehicle/{registrationNumber}";
            logger.LogInformation("Calling Vehicle Service for registration {RegistrationNumber} at {Url}", registrationNumber, url);

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Vehicle Service returned {StatusCode} for registration {Registration}", 
                    response.StatusCode, registrationNumber);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var vehicleResponse = JsonSerializer.Deserialize<VehicleServiceResponse>(json, options);
            
            return vehicleResponse?.Vehicle;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling Vehicle Service for registration {Registration}", registrationNumber);
            return null;
        }
    }
}