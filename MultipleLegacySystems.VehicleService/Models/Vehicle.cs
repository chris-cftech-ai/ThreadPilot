namespace MultipleLegacySystems.VehicleService.Models;

public record Vehicle
{
    public required string RegistrationNumber { get; init; }
    public required string Make { get; init; }
    public required string Model { get; init; }
    public required int Year { get; init; }
    public required string Color { get; init; }
    public required string VinNumber { get; init; }
}