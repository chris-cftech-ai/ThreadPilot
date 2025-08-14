namespace ThreadPilot.InsuranceService.Models;

public record PersonInsuranceDetails
{
    public required string PersonId { get; init; }
    public required string PersonName { get; init; }
    public required List<InsuranceDetails> Insurances { get; init; }
    public required decimal TotalMonthlyCost { get; init; }
}