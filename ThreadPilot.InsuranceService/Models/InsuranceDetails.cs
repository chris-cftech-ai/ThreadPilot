namespace ThreadPilot.InsuranceService.Models;

public record InsuranceDetails
{
    public required InsuranceType Type { get; init; }
    public required decimal MonthlyCost { get; init; }
    public VehicleInfo? Vehicle { get; init; }
}