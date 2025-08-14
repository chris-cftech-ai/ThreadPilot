using ThreadPilot.InsuranceService.Models;

namespace ThreadPilot.InsuranceService.Clients;

public interface IVehicleServiceClient
{
    Task<VehicleInfo?> GetVehicleAsync(string registrationNumber);
}