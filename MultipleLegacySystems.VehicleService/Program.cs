using FluentValidation;
using FluentValidation.AspNetCore;
using MultipleLegacySystems.VehicleService.Mapping;
using MultipleLegacySystems.VehicleService.Repositories;
using MultipleLegacySystems.VehicleService.Services;
using MultipleLegacySystems.VehicleService.Validators;

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
        Title = "Vehicle Service API",
        Version = "v1",
        Description = "API for managing vehicle information",
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
builder.Services.AddScoped<IVehicleRepository, InMemoryVehicleRepository>();

// Register services
builder.Services.AddScoped<IVehicleService, VehicleService>();

// Register validators
builder.Services.AddValidatorsFromAssemblyContaining<RegistrationNumberValidator>();

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