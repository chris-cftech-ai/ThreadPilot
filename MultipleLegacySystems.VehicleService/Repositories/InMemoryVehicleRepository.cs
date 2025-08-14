using MultipleLegacySystems.VehicleService.Models;

namespace MultipleLegacySystems.VehicleService.Repositories;

public class InMemoryVehicleRepository : IVehicleRepository
{
    private readonly List<Vehicle> _vehicles =
    [
        new() { RegistrationNumber = "ABC123", Make = "Toyota", Model = "Camry", Year = 2022, Color = "Blue", VinNumber = "1HGBH41JXMN109186" },
        new() { RegistrationNumber = "XYZ789", Make = "Honda", Model = "Civic", Year = 2021, Color = "Red", VinNumber = "2HGBH41JXMN109187" },
        new() { RegistrationNumber = "DEF456", Make = "Ford", Model = "F-150", Year = 2023, Color = "White", VinNumber = "3HGBH41JXMN109188" },
        new() { RegistrationNumber = "GHI789", Make = "BMW", Model = "X3", Year = 2020, Color = "Black", VinNumber = "4HGBH41JXMN109189" },
        new() { RegistrationNumber = "JKL012", Make = "Mercedes", Model = "C-Class", Year = 2022, Color = "Silver", VinNumber = "5HGBH41JXMN109190" }
    ];

    public Task<Vehicle?> GetVehicleByRegistrationNumberAsync(string registrationNumber)
    {
        var vehicle = _vehicles.FirstOrDefault(v => 
            v.RegistrationNumber.Equals(registrationNumber, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(vehicle);
    }
}