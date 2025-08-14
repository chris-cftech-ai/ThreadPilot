# ThreadPilot Integration Layer

A microservices-based integration layer between ThreadPilot core system and legacy systems, providing vehicle information and insurance management services.

## 🏗️ Architecture & Design

### System Architecture

The solution consists of two separate microservices communicating via HTTP:

```
┌─────────────────┐    HTTP   ┌─────────────────────┐
│                 │   calls   │                     │
│  Insurance      │ ────────► │  Vehicle Service    │
│  Service        │           │                     │
│  (Port 5002)    │           │  (Port 5001)        │
└─────────────────┘           └─────────────────────┘
        │                              │
        │                              │
        ▼                              ▼
┌─────────────────┐           ┌─────────────────────┐
│  Insurance      │           │  Vehicle            │
│  Repository     │           │  Repository         │
│  (In-Memory)    │           │  (In-Memory)        │
└─────────────────┘           └─────────────────────┘
```

### Key Design Decisions

1. **Microservices Pattern**: Separate services for distinct business domains (vehicles vs insurance)
2. **Repository Pattern**: Abstracted data access with in-memory implementations for demonstration
3. **HTTP Client Integration**: Insurance Service calls Vehicle Service when car insurance is present
4. **Graceful Degradation**: Insurance Service continues operating even if Vehicle Service is unavailable
5. **Problem Details (RFC 7807)**: Standardized error responses across all APIs
6. **Clean Response Models**: Response models contain only data, errors handled separately
7. **HTTP Resilience**: Polly-based retry and circuit breaker patterns for external calls
8. **Modern C# Patterns**: Property-based records, AutoMapper, structured logging

### Technology Stack

- **.NET 9.0**: Latest LTS framework with C# 13
- **ASP.NET Core**: Web API framework with built-in dependency injection
- **Problem Details (RFC 7807)**: Standardized error responses for APIs
- **FluentValidation**: Advanced input validation with automatic Problem Details integration
- **AutoMapper**: Object-to-object mapping for clean response models
- **Polly**: HTTP resilience patterns (retry, circuit breaker) for external calls
  - **Exponential Backoff**: 3 retries with 2^attempt second delays
  - **Circuit Breaker**: Opens after 3 failures, 30-second recovery time
- **Swagger/OpenAPI**: API documentation and testing interface
- **xUnit + Moq**: Unit testing framework with mocking capabilities
- **Property-based Records**: Modern C# immutable data structures
- **Structured Logging**: Consistent logging patterns across services

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- Git
- IDE (Visual Studio, VS Code, or Rider)

### Local Setup

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd ThreadPilot.Integration
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Run the Vehicle Service**:
   ```bash
   dotnet run --project MultipleLegacySystems.VehicleService
   # Runs on https://localhost:7001 or http://localhost:5001
   ```

4. **Run the Insurance Service** (in a new terminal):
   ```bash
   dotnet run --project ThreadPilot.InsuranceService
   # Runs on https://localhost:7002 or http://localhost:5002
   ```

5. **Access the APIs**:
   - Vehicle Service: http://localhost:5001/swagger
   - Insurance Service: http://localhost:5002/swagger
   - Health Checks: `/health` endpoint available on both services

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests with coverage (if coverage tools installed)
dotnet test --collect:"XPlat Code Coverage"
```

### Building for Production

```bash
# Build all projects
dotnet build --configuration Release

# Publish Vehicle Service
dotnet publish MultipleLegacySystems.VehicleService -c Release -o ./publish/vehicle

# Publish Insurance Service
dotnet publish ThreadPilot.InsuranceService -c Release -o ./publish/insurance
```

## 📋 API Reference

### Vehicle Service (Port 5001)

#### GET /api/vehicle/{registrationNumber}
Returns vehicle information by registration number.

**Example Request**:
```http
GET /api/vehicle/ABC123
```

**Example Response**:
```json
{
  "vehicle": {
    "registrationNumber": "ABC123",
    "make": "Toyota",
    "model": "Camry",
    "year": 2022,
    "color": "Blue",
    "vinNumber": "1HGBH41JXMN109186"
  }
}
```

### Insurance Service (Port 5002)

#### GET /api/insurance/{personId}
Returns all insurances for a person, with vehicle details for car insurance.

**Example Request**:
```http
GET /api/insurance/12345678901
```

**Example Response**:
```json
{
  "personInsurance": {
    "personId": "12345678901",
    "personName": "John Doe",
    "insurances": [
      {
        "type": "Pet insurance",
        "monthlyCost": 10.0,
        "vehicleRegistration": null
      },
      {
        "type": "Personal health insurance",
        "monthlyCost": 20.0,
        "vehicleRegistration": null
      },
      {
        "type": "Car insurance",
        "monthlyCost": 30.0,
        "vehicleRegistration": "ABC123"
      }
    ],
    "totalMonthlyCost": 60.0
  }
}
```

## 🛡️ Error Handling

### Error Response Format
All services use RFC 7807 Problem Details for consistent error responses:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Vehicle with registration 'INVALID' was not found",
  "instance": "GET /api/vehicle/INVALID",
  "traceId": "0HN7GLLMG5J5H:00000001"
}
```

### Validation Error Format
FluentValidation errors are automatically converted to Problem Details:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "PersonId": ["Person ID must be exactly 11 digits"]
  },
  "traceId": "0HN7GLLMG5J5H:00000002"
}
```

### HTTP Status Codes
- `200 OK`: Successful request with data
- `400 Bad Request`: Invalid input parameters
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Unexpected server errors

### Error Scenarios Handled
1. **Validation Errors**: FluentValidation automatically returns Problem Details
2. **Not Found**: 404 responses with detailed Problem Details
3. **Service Unavailability**: Graceful degradation with Polly resilience patterns
4. **Network Issues**: Automatic retries and circuit breaker protection
5. **Unhandled Exceptions**: ASP.NET Core automatically converts to Problem Details
6. **Structured Error Information**: All errors include trace IDs for debugging

### Input Validation Rules

**Person ID (Insurance Service)**:
- Must be exactly 11 numeric characters
- Required field validation
- Custom FluentValidation rule

**Vehicle Registration Number**:
- Length between 3-8 characters
- Alphanumeric characters only  
- Case-insensitive lookups in repository
- Required field validation

## 🧪 Testing Strategy

### Test Coverage (13 comprehensive tests)

**Comprehensive Test Philosophy**:
The test suite covers critical business logic, API behavior, and service-to-service communication, ensuring reliable integration between microservices.

**Test Categories**:
1. **Controller Tests** (4 tests):
   - Vehicle controller: Valid + invalid registration scenarios with full AutoMapper integration
   - Insurance controller: Valid + invalid person ID scenarios with service integration

2. **Repository Tests** (5 tests):
   - Vehicle repository: Valid + invalid scenarios including case-insensitive search
   - Insurance repository: Valid + invalid scenarios with business data validation

3. **Integration Tests** (4 tests):
   - **VehicleServiceClient Integration**: Tests HTTP communication between Insurance and Vehicle services
     - Valid registration lookup with proper response deserialization
     - Non-existent registration handling (404 responses)
     - Network error handling and graceful degradation
     - Invalid JSON response handling for robustness

4. **Service Integration Coverage**:
   - Controller tests provide integration coverage by testing full request-to-response flows
   - Service layer integration with repositories and AutoMapper
   - Error handling integration with Problem Details system
   - HTTP client integration testing with mocked responses

**Integration Test Benefits**:
- Validates service-to-service communication reliability
- Tests HTTP client configuration and error handling
- Ensures proper JSON serialization/deserialization
- Verifies resilience patterns work correctly

### Running Tests
```bash
# Run all tests (13 comprehensive tests)
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run specific test classes
dotnet test --filter "VehicleServiceTests"
dotnet test --filter "InsuranceServiceTests"
dotnet test --filter "VehicleServiceClientIntegrationTests"
```

## 🔧 Extensibility & Future Enhancements

### Service Extension Points

1. **Repository Pattern**: Easy to swap in-memory implementations with database persistence
   ```csharp
   // Replace in Program.cs:
   builder.Services.AddScoped<IVehicleRepository, SqlServerVehicleRepository>();
   ```

2. **HTTP Client Configuration**: Configurable timeouts, retry policies, and circuit breakers
3. **Authentication/Authorization**: Add JWT/OAuth2 support
4. **Caching**: Add Redis/In-Memory caching for vehicle lookups
5. **Logging**: Structured logging with Serilog or Application Insights

### API Versioning Strategy

For future API changes, implement versioning:
```csharp
[ApiController]
[Route("api/v{version:apiVersion}/vehicle")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class VehicleController : ControllerBase
```

### Configuration Management

Environment-specific configurations:
```json
{
  "VehicleService": {
    "BaseUrl": "https://vehicle-service.production.com",
    "TimeoutSeconds": 30,
    "RetryCount": 3
  }
}
```

## 🏗️ Production Considerations

### Security
- Add HTTPS enforcement
- Implement API key authentication
- Add rate limiting
- Input validation and sanitization
- SQL injection prevention (when using databases)

### Monitoring & Observability
- Health check endpoints
- Application metrics (Prometheus/AppMetrics)
- Distributed tracing (OpenTelemetry)
- Centralized logging (ELK stack)

### Scalability
- Horizontal scaling with load balancers
- Database connection pooling
- Async/await patterns for I/O operations
- Background job processing

### Reliability
- Circuit breaker pattern for external calls
- Retry policies with exponential backoff
- Graceful shutdown handling
- Database failover strategies

## 👥 Development Team Onboarding

### Code Standards
- Follow Microsoft C# coding conventions
- Use dependency injection for all services
- Implement comprehensive logging
- Write tests for all business logic
- Use async/await for I/O operations

### Development Workflow
1. Feature branches from `main`
2. Pull requests with code reviews
3. Automated testing in CI/CD
4. Integration testing before merge

### Local Development Setup
```bash
# Install recommended VS Code extensions
code --install-extension ms-dotnettools.csharp
code --install-extension ms-dotnettools.dotnet-interactive-vscode

# Set up development certificates
dotnet dev-certs https --trust
```

### Debugging
- Use `dotnet watch run` for hot reload during development
- Configure logging levels in appsettings.Development.json
- Use Swagger UI for interactive API testing

---

## 📊 Insurance Products & Pricing

| Insurance Type | Monthly Cost |
|---------------|-------------|
| Pet insurance | $10 |
| Personal health insurance | $20 |
| Car insurance | $30 |

## 📝 Sample Data

The system includes pre-populated test data:

**Vehicles**:
- ABC123: Toyota Camry 2022 (Blue)
- XYZ789: Honda Civic 2021 (Red)
- DEF456: Ford F-150 2023 (White)

**Insurance Policies**:
- Person 12345678901 (John Doe): Pet + Health + Car insurance
- Person 98765432109 (Jane Smith): Health insurance only
- Person 55566677788 (Mike Johnson): Pet + Car insurance

*Built with ❤️ for ThreadPilot Integration Layer*

**Personal reflections**:
- o Any similar project or experience you’ve had in the past.
- At current customer we also concatenate data from multiple sources and one endpoint returns "new data"
- o What was challenging or interesting in this assignment.
- It was a very fun task trying to get ai to get what you want. As always things
- takes little longer than usual when you try going through the requirement list
- and iteration was necessary to get desired results also run out of tokens was another issue,
- being on vacation with kids at home :)
- o What you would improve or extend if you had more time.
- I would add authentication / authorization (now it is public), integration tests should go all the way down the pipe now i simplified.
- Change to a real storage like sql, now it is only mocking. Caching api calls.
**Brief section on how you would approach onboarding
  or enabling other developers to work with your solution.**
- Good swagger documentation och git repo with example code.

**Discussion of API versioning and future extensibility**
- Använde versionsprefix i URL:en
- Vid breakingchanges kommer man med ny version
- Applicera lägg till strategi så långt det går
- Vid övergång kör man gamla och ny api parallellt en tid