using MultipleLegacySystems.VehicleService.Models;
using MultipleLegacySystems.VehicleService.Repositories;

namespace MultipleLegacySystems.VehicleService.Services;

public class VehicleService(
    IVehicleRepository vehicleRepository)
    : IVehicleService
{
    public async Task<Vehicle?> GetVehicleByRegistrationNumberAsync(string registrationNumber) => 
        await vehicleRepository.GetVehicleByRegistrationNumberAsync(registrationNumber);
}