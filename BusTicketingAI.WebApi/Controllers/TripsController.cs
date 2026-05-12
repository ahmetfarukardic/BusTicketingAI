using BusTicketingAI.Application.Features.Trips.Queries.GetTripDetails;
using BusTicketingAI.Application.Features.Trips.Queries.SearchTrips;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketingAI.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TripsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TripsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTrips([FromQuery] SearchTripsQuery query)
    {
        var trips = await _mediator.Send(query);
        return Ok(trips);
    }

    [HttpGet("{tripId}/details")]
    public async Task<IActionResult> GetTripDetails(Guid tripId, CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetTripDetailsQuery(tripId);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
