using MultipleLegacySystems.VehicleService.Models;

namespace MultipleLegacySystems.VehicleService.Services;

public interface IVehicleService
{
    Task<Vehicle?> GetVehicleByRegistrationNumberAsync(string registrationNumber);
}