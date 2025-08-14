namespace ThreadPilot.InsuranceService.Models;

public record PersonInsurance
{
    public required string PersonId { get; init; }
    public required string PersonName { get; init; }
    public required List<Insurance> Insurances { get; init; }
    public required decimal TotalMonthlyCost { get; init; }
}