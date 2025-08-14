using AutoMapper;
using Moq;
using ThreadPilot.InsuranceService.Controllers;
using ThreadPilot.InsuranceService.Mapping;
using ThreadPilot.InsuranceService.Models;
using ThreadPilot.InsuranceService.Responses;
using ThreadPilot.InsuranceService.Services;
using ThreadPilot.InsuranceService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ThreadPilot.Tests;

public class InsuranceServiceTests
{
    private readonly Mock<IInsuranceService> _mockInsuranceService;
    private readonly InsuranceController _controller;

    public InsuranceServiceTests()
    {
        _mockInsuranceService = new Mock<IInsuranceService>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();
        _controller = new InsuranceController(_mockInsuranceService.Object, mapper);
    }

    [Fact]
    public async Task GetPersonInsurances_WithValidPersonId_ReturnsInsurances()
    {
        // Arrange
        var personId = "12345678901";
        var insurances = new List<InsuranceDetails>
        {
            new() { Type = InsuranceType.PetInsurance, MonthlyCost = 10m },
            new() { Type = InsuranceType.PersonalHealthInsurance, MonthlyCost = 20m }
        };
        var personInsurance = new PersonInsuranceDetails
        {
            PersonId = personId,
            PersonName = "John Doe",
            Insurances = insurances,
            TotalMonthlyCost = 30m
        };
        
        _mockInsuranceService.Setup(x => x.GetPersonInsurancesAsync(personId))
                            .ReturnsAsync(personInsurance);

        // Act
        var result = await _controller.GetPersonInsurances(personId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<InsuranceResponse>(okResult.Value);
        Assert.NotNull(response);
        Assert.NotNull(response.PersonInsuranceDetails);
        Assert.Equal(personId, response.PersonInsuranceDetails.PersonId);
        Assert.Equal(2, response.PersonInsuranceDetails.Insurances.Count);
    }

    [Fact]
    public async Task GetPersonInsurances_WithNonExistentPersonId_ReturnsNotFound()
    {
        // Arrange
        var personId = "NOTFOUND";
        
        _mockInsuranceService.Setup(x => x.GetPersonInsurancesAsync(personId))
                            .ReturnsAsync((PersonInsuranceDetails?)null);

        // Act
        var result = await _controller.GetPersonInsurances(personId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("No insurances found", notFoundResult.Value?.ToString());
    }

}

public class InMemoryInsuranceRepositoryTests
{
    private readonly InMemoryInsuranceRepository _repository;

    public InMemoryInsuranceRepositoryTests()
    {
        _repository = new InMemoryInsuranceRepository();
    }

    [Fact]
    public async Task GetPersonInsuranceAsync_WithExistingPersonId_ReturnsInsurance()
    {
        // Act
        var result = await _repository.GetPersonInsuranceAsync("12345678901");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("12345678901", result.PersonId);
        Assert.Equal("John Doe", result.PersonName);
        Assert.Equal(3, result.Insurances.Count);
        Assert.Equal(60m, result.TotalMonthlyCost);
    }

    [Fact]
    public async Task GetPersonInsuranceAsync_WithNonExistingPersonId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetPersonInsuranceAsync("NOTFOUND");

        // Assert
        Assert.Null(result);
    }
}