using BusTicketingAI.Application.Features.WalletTransactions.Commands;
using BusTicketingAI.Application.Features.WalletTransactions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BusTicketingAI.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WalletsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WalletsController(IMediator mediator) => _mediator = mediator;

    private Guid GetUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(userIdString!);
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWalletBalanceQuery(GetUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWalletTransactionQuery(GetUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{userId}/balance")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserBalance(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWalletBalanceQuery(userId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("adjust-balance")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdjustBalance([FromBody] AdjustWalletBalanceCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}