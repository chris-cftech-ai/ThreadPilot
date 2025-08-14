using ThreadPilot.InsuranceService.Models;

namespace ThreadPilot.InsuranceService.Repositories;

public class InMemoryInsuranceRepository : IInsuranceRepository
{
    private readonly Dictionary<string, PersonInsurance> _insuranceData = new()
    {
        {
            "12345678901",
            new PersonInsurance
            {
                PersonId = "12345678901",
                PersonName = "John Doe",
                Insurances = new List<Insurance>
                {
                    new() { Type = InsuranceType.PetInsurance, MonthlyCost = 10m },
                    new() { Type = InsuranceType.PersonalHealthInsurance, MonthlyCost = 20m },
                    new() { Type = InsuranceType.CarInsurance, MonthlyCost = 30m, VehicleRegistration = "ABC123" }
                },
                TotalMonthlyCost = 60m
            }
        },
        {
            "98765432109",
            new PersonInsurance
            {
                PersonId = "98765432109",
                PersonName = "Jane Smith",
                Insurances = new List<Insurance>
                {
                    new() { Type = InsuranceType.PersonalHealthInsurance, MonthlyCost = 20m }
                },
                TotalMonthlyCost = 20m
            }
        },
        {
            "55566677788",
            new PersonInsurance
            {
                PersonId = "55566677788",
                PersonName = "Mike Johnson",
                Insurances = new List<Insurance>
                {
                    new() { Type = InsuranceType.PetInsurance, MonthlyCost = 10m },
                    new() { Type = InsuranceType.CarInsurance, MonthlyCost = 30m, VehicleRegistration = "XYZ789" }
                },
                TotalMonthlyCost = 40m
            }
        }
    };

    public Task<PersonInsurance?> GetPersonInsuranceAsync(string personId)
    {
        _insuranceData.TryGetValue(personId, out var insurance);
        return Task.FromResult(insurance);
    }
}