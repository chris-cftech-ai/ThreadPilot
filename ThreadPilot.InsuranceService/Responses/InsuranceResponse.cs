using ThreadPilot.InsuranceService.Models;

namespace ThreadPilot.InsuranceService.Responses;

public record InsuranceResponse
{
    public PersonInsuranceDetails? PersonInsuranceDetails { get; init; }
}