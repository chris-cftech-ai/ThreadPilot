using ThreadPilot.InsuranceService.Models;

namespace ThreadPilot.InsuranceService.Responses;

public record VehicleServiceResponse
{
    public VehicleInfo? Vehicle { get; init; }
}