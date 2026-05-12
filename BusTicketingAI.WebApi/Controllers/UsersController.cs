using BusTicketingAI.Application.Features.Users;
using BusTicketingAI.Application.Features.Users.Commands;
using BusTicketingAI.Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketingAI.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] CreateUserDto user)
    {
        var userId = await _mediator.Send(new CreateUserCommand(user.FirstName, user.LastName, user.Email, user.Password, user.Role, user.CompanyId));
        return Ok(userId);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto user)
    {
        await _mediator.Send(new UpdateUserCommand(user.Id, user.FirstName, user.LastName, user.Email, user.Role, user.CompanyId));
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveUser([FromRoute] Guid id)
    {
        await _mediator.Send(new DeleteUserCommand(id));
        return Ok();
    }
}
