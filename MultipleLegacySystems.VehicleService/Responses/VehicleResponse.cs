using MultipleLegacySystems.VehicleService.Models;

namespace MultipleLegacySystems.VehicleService.Responses;

public record VehicleResponse
{
    public Vehicle? Vehicle { get; init; }
}