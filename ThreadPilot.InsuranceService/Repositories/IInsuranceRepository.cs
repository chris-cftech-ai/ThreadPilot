using ThreadPilot.InsuranceService.Models;

namespace ThreadPilot.InsuranceService.Repositories;

public interface IInsuranceRepository
{
    Task<PersonInsurance?> GetPersonInsuranceAsync(string personId);
}