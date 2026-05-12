using BusTicketingAI.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BusTicketingAI.Application.Features.Users.Commands;

public record UpdateUserCommand(Guid Id, string FirstName, string LastName, string Email, string Role, int? CompanyId) : IRequest;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly UserManager<AppUser> _userManager;
    public UpdateUserCommandHandler(UserManager<AppUser> userManager) => _userManager = userManager;

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString()) ?? throw new Exception("Güncellenecek kullanıcı bulunamadı!");
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.CompanyId = request.CompanyId;

        user.Email = request.Email;
        user.UserName = request.Email;

        var userRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, userRoles);
        await _userManager.AddToRoleAsync(user, request.Role);

        await _userManager.UpdateAsync(user);
    }
}
