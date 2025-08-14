using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ThreadPilot.InsuranceService.Responses;
using ThreadPilot.InsuranceService.Services;
using System.ComponentModel.DataAnnotations;

namespace ThreadPilot.InsuranceService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InsuranceController(IInsuranceService insuranceService, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Gets insurance information for a person by their ID
    /// </summary>
    /// <param name="personId">The person's identification number</param>
    /// <returns>Person's insurance information if found</returns>
    /// <response code="200">Insurance information found and returned</response>
    /// <response code="404">No insurance found for the person</response>
    /// <response code="400">Invalid person ID format</response>
    [HttpGet("{personId}")]
    [ProducesResponseType(typeof(InsuranceResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> GetPersonInsurances(
        [Required] string personId)
    {
        var personInsuranceDetails = await insuranceService.GetPersonInsurancesAsync(personId);
        
        if (personInsuranceDetails == null)
        {
            return NotFound($"No insurances found for person {personId}");
        }
        
        var response = mapper.Map<InsuranceResponse>(personInsuranceDetails);
        return Ok(response);
    }
}