using AutoMapper;
using Moq;
using MultipleLegacySystems.VehicleService.Controllers;
using MultipleLegacySystems.VehicleService.Mapping;
using MultipleLegacySystems.VehicleService.Models;
using MultipleLegacySystems.VehicleService.Responses;
using MultipleLegacySystems.VehicleService.Services;
using MultipleLegacySystems.VehicleService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ThreadPilot.Tests;

public class VehicleServiceTests
{
    private readonly Mock<IVehicleService> _mockVehicleService;
    private readonly VehicleController _controller;

    public VehicleServiceTests()
    {
        _mockVehicleService = new Mock<IVehicleService>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();
        _controller = new VehicleController(_mockVehicleService.Object, mapper);
    }

    [Fact]
    public async Task GetVehicle_WithValidRegistration_ReturnsVehicle()
    {
        // Arrange
        var registration = "ABC123";
        var expectedVehicle = new Vehicle
        {
            RegistrationNumber = registration,
            Make = "Toyota",
            Model = "Camry",
            Year = 2022,
            Color = "Blue",
            VinNumber = "1HGBH41JXMN109186"
        };
        
        _mockVehicleService.Setup(x => x.GetVehicleByRegistrationNumberAsync(It.IsAny<string>()))
                          .ReturnsAsync(expectedVehicle);

        // Act
        var result = await _controller.GetVehicle(registration);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<VehicleResponse>(okResult.Value);
        Assert.NotNull(response);
        Assert.NotNull(response.Vehicle);
        Assert.Equal(registration, response.Vehicle.RegistrationNumber);
        Assert.Equal("Toyota", response.Vehicle.Make);
    }

    [Fact]
    public async Task GetVehicle_WithNonExistentRegistration_ReturnsNotFound()
    {
        // Arrange
        var registration = "NOTFOUND";
        
        _mockVehicleService.Setup(x => x.GetVehicleByRegistrationNumberAsync(It.IsAny<string>()))
                          .ReturnsAsync((Vehicle?)null);

        // Act
        var result = await _controller.GetVehicle(registration);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("not found", notFoundResult.Value?.ToString());
    }

}

public class InMemoryVehicleRepositoryTests
{
    private readonly InMemoryVehicleRepository _repository;

    public InMemoryVehicleRepositoryTests()
    {
        _repository = new InMemoryVehicleRepository();
    }

    [Fact]
    public async Task GetVehicleByRegistrationAsync_WithExistingRegistration_ReturnsVehicle()
    {
        // Act
        var result = await _repository.GetVehicleByRegistrationNumberAsync("ABC123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ABC123", result.RegistrationNumber);
        Assert.Equal("Toyota", result.Make);
        Assert.Equal("Camry", result.Model);
    }

    [Fact]
    public async Task GetVehicleByRegistrationAsync_WithNonExistingRegistration_ReturnsNull()
    {
        // Act
        var result = await _repository.GetVehicleByRegistrationNumberAsync("NOTFOUND");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetVehicleByRegistrationAsync_IsCaseInsensitive()
    {
        // Act
        var result = await _repository.GetVehicleByRegistrationNumberAsync("abc123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ABC123", result.RegistrationNumber);
    }
}