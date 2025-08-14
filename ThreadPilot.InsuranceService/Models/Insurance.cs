namespace ThreadPilot.InsuranceService.Models;

public record Insurance
{
    public required InsuranceType Type { get; init; }
    public required decimal MonthlyCost { get; init; }
    public string? VehicleRegistration { get; init; }
    public VehicleInfo? Vehicle { get; init; }
}