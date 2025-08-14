using MultipleLegacySystems.VehicleService.Models;

namespace MultipleLegacySystems.VehicleService.Repositories;

public interface IVehicleRepository
{
    Task<Vehicle?> GetVehicleByRegistrationNumberAsync(string registrationNumber);
}