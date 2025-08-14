using ThreadPilot.InsuranceService.Models;

namespace ThreadPilot.InsuranceService.Services;

public interface IInsuranceService
{
    Task<PersonInsuranceDetails?> GetPersonInsurancesAsync(string personId);
}