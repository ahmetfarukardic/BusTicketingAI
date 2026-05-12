using BusTicketingAI.Application.Features.Tickets.Commands.CancelTicket;
using BusTicketingAI.Application.Features.Tickets.Commands.CheckoutTicket;
using BusTicketingAI.Application.Features.Tickets.Commands.LockSeats;
using BusTicketingAI.Application.Features.Tickets.Queries.GetPassengerTicket;
using BusTicketingAI.Application.Features.Users.Queries.GetUserProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BusTicketingAI.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetMyTickets()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized("Kullanıcı kimliği doğrulanamadı.");

        var userId = Guid.Parse(userIdClaim);
        var response = await _mediator.Send(new GetPassengerTicketQuery(userId));
        return Ok(response);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized("Kullanıcı kimliği doğrulanamadı.");

        var userId = Guid.Parse(userIdClaim);
        var response = await _mediator.Send(new GetUserProfileQuery(userId));
        return Ok(response);
    }

    [HttpPost("lock-seats")]
    public async Task<IActionResult> LockSeats([FromBody] LockSeatsCommand request)
    {
        var result = await _mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(new { result.Message });

        return Ok(new { Message = "Koltuklar 5 dakikalığına sizin için rezerve edildi." });
    }

    [HttpPost("unlock-seats")]
    public async Task<IActionResult> UnlockSeats([FromBody] UnlockSeatsCommand request)
    {
        await _mediator.Send(request);
        return Ok();
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutCommand request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid? currentUserId = !string.IsNullOrEmpty(userIdClaim) ? Guid.Parse(userIdClaim) : null;

        var secureCommand = request with { UserId = currentUserId };
        var result = await _mediator.Send(secureCommand);
        return Ok(new { Message = "Ödeme başarılı, biletleriniz kesildi!", TripId = result });
    }

    [HttpPatch("profile/{ticketId}/cancel")]
    public async Task<IActionResult> CancelMyTicket(Guid ticketId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized("Kullanıcı kimliği doğrulanamadı.");

        var userId = Guid.Parse(userIdClaim);

        var command = new CancelMyTicketCommand(ticketId, userId);
        await _mediator.Send(command);
        return Ok(new { Message = "Biletiniz başarıyla iptal edildi." });
    }
}