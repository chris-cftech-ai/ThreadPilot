using ThreadPilot.InsuranceService.Clients;
using ThreadPilot.InsuranceService.Models;
using ThreadPilot.InsuranceService.Repositories;

namespace ThreadPilot.InsuranceService.Services;

public class InsuranceService(
    IInsuranceRepository insuranceRepository,
    IVehicleServiceClient vehicleServiceClient,
    ILogger<InsuranceService> logger)
    : IInsuranceService
{
    public async Task<PersonInsuranceDetails?> GetPersonInsurancesAsync(string personId)
    {
        logger.LogInformation("Looking up insurances for person {PersonId}", personId);

        var personInsurance = await insuranceRepository.GetPersonInsuranceAsync(personId);

        if (personInsurance == null)
        {
            logger.LogInformation("No insurances found for person {PersonId}", personId);
            return null;
        }

        var insuranceDetails = new List<InsuranceDetails>();

        foreach (var insurance in personInsurance.Insurances)
        {
            insuranceDetails.Add(new InsuranceDetails
            {
                Type = insurance.Type,
                MonthlyCost = insurance.MonthlyCost,
                Vehicle = await GetVehicleInfoAsync(insurance)
            });
        }

        var personInsuranceDetails = new PersonInsuranceDetails
        {
            PersonId = personInsurance.PersonId,
            PersonName = personInsurance.PersonName,
            Insurances = insuranceDetails,
            TotalMonthlyCost = personInsurance.TotalMonthlyCost
        };

        logger.LogInformation("Successfully retrieved {InsuranceCount} insurances for person {PersonId}", insuranceDetails.Count, personId);

        return personInsuranceDetails;
    }

    private async Task<VehicleInfo?> GetVehicleInfoAsync(Insurance insurance)
    {
        if (insurance.Type != InsuranceType.CarInsurance || string.IsNullOrEmpty(insurance.VehicleRegistration))
        {
            return null;
        }

        logger.LogInformation("Fetching vehicle details for registration {VehicleRegistration}", insurance.VehicleRegistration);

        return await vehicleServiceClient.GetVehicleAsync(insurance.VehicleRegistration);
    }
}