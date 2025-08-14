using FluentValidation;
using FluentValidation.AspNetCore;
using ThreadPilot.InsuranceService.Clients;
using ThreadPilot.InsuranceService.Configuration;
using ThreadPilot.InsuranceService.Mapping;
using ThreadPilot.InsuranceService.Repositories;
using ThreadPilot.InsuranceService.Services;
using ThreadPilot.InsuranceService.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddFluentValidationAutoValidation(config =>
{
    config.DisableDataAnnotationsValidation = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Insurance Service API",
        Version = "v1",
        Description = "API for managing insurance information",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "ThreadPilot Team"
        }
    });
    
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});
builder.Services.AddHealthChecks();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register repositories
builder.Services.AddScoped<IInsuranceRepository, InMemoryInsuranceRepository>();

// Register HTTP clients with Polly policies
builder.Services.AddHttpClient<IVehicleServiceClient, VehicleServiceClient>(client =>
{
    var baseUrl = builder.Configuration["VehicleService:BaseUrl"] ?? "http://localhost:5001";
    client.BaseAddress = new Uri($"{baseUrl}/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddPolicyHandler((serviceProvider, request) => 
{
    var logger = serviceProvider.GetService<ILogger<VehicleServiceClient>>();
    return HttpPolicyConfiguration.GetRetryPolicy(logger);
})
.AddPolicyHandler((serviceProvider, request) => 
{
    var logger = serviceProvider.GetService<ILogger<VehicleServiceClient>>();
    return HttpPolicyConfiguration.GetCircuitBreakerPolicy(logger);
});

// Register services
builder.Services.AddScoped<IInsuranceService, InsuranceService>();

// Register validators
builder.Services.AddValidatorsFromAssemblyContaining<PersonIdValidator>();

var app = builder.Build();

// Add built-in exception handling
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();