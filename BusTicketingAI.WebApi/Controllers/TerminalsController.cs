using BusTicketingAI.Application.Features.Terminals.Queries.GetAllTerminals;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketingAI.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TerminalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TerminalsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetTerminals()
    {
        var result = await _mediator.Send(new GetAllTerminalsQuery());
        return Ok(result);
    }
}
