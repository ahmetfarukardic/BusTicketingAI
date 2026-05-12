using BusTicketingAI.Application.Features.Cities.Queries.GetAllCities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketingAI.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCities()
    {
        var result = await _mediator.Send(new GetAllCitiesQuery());
        return Ok(result);
    }
}
