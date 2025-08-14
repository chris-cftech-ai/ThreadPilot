using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using ThreadPilot.InsuranceService.Clients;
using ThreadPilot.InsuranceService.Models;
using ThreadPilot.InsuranceService.Responses;

namespace ThreadPilot.Tests;

public class VehicleServiceClientIntegrationTests
{
    [Fact]
    public async Task VehicleServiceClient_GetVehicle_WithValidRegistration_ReturnsVehicle()
    {
        // Arrange
        var expectedVehicle = new VehicleInfo
        {
            RegistrationNumber = "ABC123",
            Make = "Toyota",
            Model = "Camry",
            Year = 2022,
            Color = "Blue",
            VinNumber = "1HGBH41JXMN109186"
        };

        var vehicleResponse = new VehicleServiceResponse { Vehicle = expectedVehicle };
        var json = JsonSerializer.Serialize(vehicleResponse);
        
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json)
            });

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://localhost:5001/api/")
        };

        var logger = new NullLogger<VehicleServiceClient>();
        var vehicleClient = new VehicleServiceClient(httpClient, logger);

        // Act
        var result = await vehicleClient.GetVehicleAsync("ABC123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ABC123", result.RegistrationNumber);
        Assert.Equal("Toyota", result.Make);
        Assert.Equal("Camry", result.Model);
    }

    [Fact]
    public async Task VehicleServiceClient_GetVehicle_WithNonExistentRegistration_ReturnsNull()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://localhost:5001/api/")
        };

        var logger = new NullLogger<VehicleServiceClient>();
        var vehicleClient = new VehicleServiceClient(httpClient, logger);

        // Act
        var result = await vehicleClient.GetVehicleAsync("NOTFOUND");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task VehicleServiceClient_GetVehicle_WithHttpException_ReturnsNull()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://localhost:5001/api/")
        };

        var logger = new NullLogger<VehicleServiceClient>();
        var vehicleClient = new VehicleServiceClient(httpClient, logger);

        // Act
        var result = await vehicleClient.GetVehicleAsync("ABC123");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task VehicleServiceClient_GetVehicle_WithInvalidJson_ReturnsNull()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("invalid json")
            });

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://localhost:5001/api/")
        };

        var logger = new NullLogger<VehicleServiceClient>();
        var vehicleClient = new VehicleServiceClient(httpClient, logger);

        // Act
        var result = await vehicleClient.GetVehicleAsync("ABC123");

        // Assert
        Assert.Null(result);
    }
}