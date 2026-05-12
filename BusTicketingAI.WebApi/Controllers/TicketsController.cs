using BusTicketingAI.Application.Features.Tickets.Commands.SellTicket;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketingAI.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "CompanyStaff")]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("sell")]
    public async Task<IActionResult> SellTicket([FromBody] SellTicketRequestDto request)
    {
        var companyIdClaim = User.FindFirst("CompanyId");
        if (companyIdClaim == null || !int.TryParse(companyIdClaim.Value, out int companyId))
        {
            return Forbid();
        }

        var command = new SellTicketCommand(
            request.TripId,
            request.SeatNumber,
            request.PassengerName,
            request.PassengerTC,
            request.Price,
            request.Gender,
            companyId
        );
        var ticketId = await _mediator.Send(command);

        return Ok(new { Id =  ticketId, message = "Bilet başarıyla kesildi ve sisteme işlendi!" });
    }
}
