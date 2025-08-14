using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MultipleLegacySystems.VehicleService.Responses;
using MultipleLegacySystems.VehicleService.Services;
using System.ComponentModel.DataAnnotations;

namespace MultipleLegacySystems.VehicleService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VehicleController(IVehicleService vehicleService, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Gets vehicle information by registration number
    /// </summary>
    /// <param name="registrationNumber">The vehicle registration number</param>
    /// <returns>Vehicle information if found</returns>
    /// <response code="200">Vehicle found and returned</response>
    /// <response code="404">Vehicle not found</response>
    /// <response code="400">Invalid registration number format</response>
    [HttpGet("{registrationNumber}")]
    [ProducesResponseType(typeof(VehicleResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> GetVehicle(
        [Required] string registrationNumber)
    {
        var vehicle = await vehicleService.GetVehicleByRegistrationNumberAsync(registrationNumber);
        
        if (vehicle == null)
        {
            return NotFound($"Vehicle with registration {registrationNumber} not found");
        }
        
        var response = mapper.Map<VehicleResponse>(vehicle);
        return Ok(response);
    }
}