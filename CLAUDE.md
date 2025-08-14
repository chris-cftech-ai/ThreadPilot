# CLAUDE.md - ThreadPilot Integration Layer

## 🤖 AI Assistant Context

This document provides context for AI assistants working on the ThreadPilot Integration Layer codebase.

## 📁 Project Structure

```
ThreadPilot/
├── MultipleLegacySystems.VehicleService/     # Vehicle microservice
│   ├── Controllers/VehicleController.cs      # Vehicle API endpoints
│   ├── Models/Vehicle.cs                     # Vehicle domain model
│   ├── Services/VehicleService.cs            # Vehicle business logic
│   ├── Repositories/InMemoryVehicleRepository.cs  # Data access layer
│   ├── Validators/RegistrationNumberValidator.cs   # Input validation
│   └── Program.cs                            # Service configuration
├── ThreadPilot.InsuranceService/             # Insurance microservice
│   ├── Controllers/InsuranceController.cs    # Insurance API endpoints
│   ├── Models/                               # Domain models
│   │   ├── Insurance.cs, InsuranceType.cs    # Business models
│   │   └── PersonInsurance.cs, VehicleInfo.cs
│   ├── Services/InsuranceService.cs          # Business logic
│   ├── Clients/VehicleServiceClient.cs       # HTTP client for Vehicle service
│   ├── Configuration/HttpPolicyConfiguration.cs  # Polly resilience policies
│   ├── Repositories/InMemoryInsuranceRepository.cs # Data access
│   ├── Validators/PersonIdValidator.cs       # Input validation
│   └── Program.cs                            # Service configuration
├── ThreadPilot.Tests/                        # Test project
│   ├── InsuranceServiceTests.cs              # Unit tests for Insurance service
│   ├── VehicleServiceTests.cs                # Unit tests for Vehicle service
│   ├── IntegrationTests.cs                   # HTTP client integration tests
│   └── ThreadPilot.Tests.csproj              # Test project configuration
├── ThreadPilot.Integration.sln               # Solution file
└── README.md                                 # Main documentation
```

## 🎯 Business Domain

### Core Concepts
- **Vehicle Service**: Manages vehicle information by registration number
- **Insurance Service**: Manages person insurance policies with optional vehicle integration
- **Service Integration**: Insurance service calls Vehicle service for car insurance details

### Data Models
- **Vehicle**: Registration number, make, model, year, color, VIN
- **Insurance**: Type (Pet/Health/Car), monthly cost, optional vehicle registration
- **Person**: ID, name, list of insurances, total monthly cost

## 🏗️ Architecture Patterns

### Microservices Design
- **Domain Separation**: Vehicles vs Insurance business domains
- **HTTP Communication**: REST APIs with JSON payloads
- **Graceful Degradation**: Services continue operating if dependencies fail
- **Port Isolation**: Vehicle (5001/7001), Insurance (5002/7002)

### Code Architecture
- **Repository Pattern**: `IVehicleRepository`, `IInsuranceRepository` with in-memory implementations
- **Service Layer**: Business logic in `VehicleService`, `InsuranceService`
- **Controller Layer**: API endpoints with proper HTTP status codes
- **Response Models**: Clean DTOs separate from domain models

### Key Technologies
- **.NET 9.0**: Latest framework with C# 13 features
- **ASP.NET Core**: Web API framework
- **AutoMapper**: Object-to-object mapping
- **FluentValidation**: Input validation with automatic Problem Details
- **Polly**: HTTP resilience (retry, circuit breaker)
- **xUnit + Moq**: Testing framework

## 🔧 Configuration Patterns

### HTTP Client Setup
```csharp
// In Program.cs - Insurance Service
builder.Services.AddHttpClient<IVehicleServiceClient, VehicleServiceClient>(client =>
{
    var baseUrl = builder.Configuration["VehicleService:BaseUrl"] ?? "http://localhost:5001";
    client.BaseAddress = new Uri($"{baseUrl}/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddPolicyHandler(HttpPolicyConfiguration.GetRetryPolicy(logger))
.AddPolicyHandler(HttpPolicyConfiguration.GetCircuitBreakerPolicy(logger));
```

### Polly Resilience Policies
- **Retry Policy**: 3 attempts with exponential backoff (2^attempt seconds)
- **Circuit Breaker**: Opens after 3 failures, 30-second recovery time
- **Comprehensive Logging**: All retry attempts and circuit breaker events logged

### Dependency Injection
- **Scoped Services**: Controllers, services, repositories
- **Typed HTTP Clients**: Configured with base URLs and policies
- **Auto-Registration**: Validators and AutoMapper profiles

## 🧪 Testing Strategy

### Test Categories (13 tests total)
1. **Controller Tests** (4): Full request-response integration with AutoMapper
2. **Repository Tests** (5): Data access layer with business logic validation
3. **Integration Tests** (4): HTTP client communication between services

### Test Patterns
- **Mocking**: `Mock<IService>` for unit testing
- **HTTP Mocking**: `Mock<HttpMessageHandler>` for HTTP client testing
- **Test Data**: Realistic sample data with cross-service consistency
- **Error Scenarios**: Network failures, invalid responses, not found cases

### Running Tests
```bash
dotnet test                                    # All tests
dotnet test --filter "VehicleServiceTests"    # Specific class
dotnet test --verbosity normal                # Detailed output
```

## 🛡️ Error Handling

### Problem Details (RFC 7807)
- **Consistent Format**: All APIs return standardized error responses
- **Automatic Conversion**: FluentValidation errors become Problem Details
- **Trace IDs**: Every error includes traceId for debugging

### Graceful Degradation
- **HTTP Client Failures**: Return null instead of throwing exceptions
- **Service Integration**: Insurance service works without Vehicle service
- **Logging**: All failures logged with context information

## 📝 Validation Rules

### Person ID (Insurance Service)
- **Format**: Exactly 11 numeric characters
- **Validator**: `PersonIdValidator` with FluentValidation

### Registration Number (Vehicle Service)
- **Format**: 3-8 alphanumeric characters
- **Validator**: `RegistrationNumberValidator`
- **Case Insensitive**: Repository performs case-insensitive lookups

## 🔍 Development Patterns

### Async/Await
- **All Controllers**: Async action methods
- **Repository Methods**: Async signatures for future database integration
- **HTTP Clients**: Async HTTP calls with proper cancellation token support

### Logging Patterns
```csharp
logger.LogInformation("Calling Vehicle Service for registration {RegistrationNumber} at {Url}", 
    registrationNumber, url);
logger.LogWarning("Vehicle Service returned {StatusCode} for registration {Registration}", 
    response.StatusCode, registrationNumber);
```

### Configuration Binding
- **Type-Safe**: Strong-typed configuration classes
- **Environment-Specific**: appsettings.json with Development override
- **Default Values**: Fallback values for missing configuration

## 🚀 Development Commands

### Running Services
```bash
# Terminal 1 - Vehicle Service
dotnet run --project MultipleLegacySystems.VehicleService

# Terminal 2 - Insurance Service  
dotnet run --project ThreadPilot.InsuranceService
```

### Build and Test
```bash
dotnet restore                           # Restore dependencies
dotnet build                            # Build all projects
dotnet test                             # Run all tests
dotnet build --configuration Release   # Release build
```

### API Testing
- **Swagger UI**: Available at `/swagger` in development
- **Health Checks**: Available at `/health` for both services
- **HTTP Files**: Visual Studio .http files for manual testing

## 📊 Sample Data

### Vehicles (in VehicleRepository)
- `ABC123`: Toyota Camry 2022 (Blue)
- `XYZ789`: Honda Civic 2021 (Red)  
- `DEF456`: Ford F-150 2023 (White)

### Insurance Policies (in InsuranceRepository)
- `12345678901` (John Doe): Pet + Health + Car insurance
- `98765432109` (Jane Smith): Health insurance only
- `55566677788` (Mike Johnson): Pet + Car insurance

## 🎛️ Configuration Values

### Default Settings
- **HTTP Timeout**: 30 seconds
- **Retry Count**: 3 attempts
- **Circuit Breaker**: 3 failures trigger, 30s recovery
- **Base URLs**: localhost:5001 (Vehicle), localhost:5002 (Insurance)

### Environment Variables
- `VehicleService__BaseUrl`: Override Vehicle service URL
- `ASPNETCORE_ENVIRONMENT`: Set to Development/Production

## 🔧 Extension Points

### Repository Replacement
```csharp
// Easy database integration
builder.Services.AddScoped<IVehicleRepository, SqlServerVehicleRepository>();
builder.Services.AddScoped<IInsuranceRepository, EntityFrameworkInsuranceRepository>();
```

### Additional HTTP Policies
- **Timeout Policy**: For request-level timeouts
- **Rate Limiting**: For API throttling
- **Caching**: For GET request caching

### Authentication
- **JWT Bearer**: For API security
- **API Keys**: For service-to-service auth
- **OAuth2**: For user authentication

## 📋 Common Tasks

### Adding New Insurance Type
1. Update `InsuranceType` enum
2. Add validation if needed
3. Update test data
4. Update documentation

### Adding New Vehicle Property
1. Update `Vehicle` record
2. Update mapping profile
3. Update repository seed data
4. Add tests for new property

### Adding New API Endpoint
1. Add controller action
2. Add input validation
3. Add service method
4. Add repository method
5. Write tests
6. Update Swagger documentation

## 🐛 Troubleshooting

### Common Issues
- **Port Conflicts**: Change ports in launchSettings.json
- **Service Communication**: Check VehicleService:BaseUrl configuration
- **Test Failures**: Ensure both services can start on their ports
- **Polly Policies**: Check logs for retry/circuit breaker activity

### Debugging Tips
- **Structured Logging**: Use template parameters for searchable logs
- **Health Checks**: Verify service health at `/health`
- **Swagger UI**: Test APIs interactively
- **HTTP Files**: Use .http files for repeatable API testing

## 🎯 Code Quality Guidelines

### C# Patterns
- **Records**: Use for immutable data models
- **Nullable Reference Types**: Enabled project-wide
- **Async/Await**: For all I/O operations
- **Dependency Injection**: Constructor injection preferred

### Testing Patterns
- **AAA Pattern**: Arrange, Act, Assert
- **One Assertion**: Per test method where possible
- **Descriptive Names**: Test method names describe the scenario
- **Test Data**: Use realistic, consistent sample data

---

*This document should be updated when significant architectural changes are made to the codebase.*