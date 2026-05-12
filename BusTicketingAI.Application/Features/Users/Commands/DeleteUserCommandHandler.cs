using BusTicketingAI.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BusTicketingAI.Application.Features.Users.Commands;

public record DeleteUserCommand(Guid Id) : IRequest;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly UserManager<AppUser> _userManager;
    public DeleteUserCommandHandler(UserManager<AppUser> userManager) => _userManager = userManager;

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString()) ?? throw new Exception("Silinecek kullanici bulunamadi!");
        await _userManager.DeleteAsync(user);
    }
}
