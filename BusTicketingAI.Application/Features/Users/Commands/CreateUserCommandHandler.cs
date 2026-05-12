using BusTicketingAI.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BusTicketingAI.Application.Features.Users.Commands;

public record CreateUserCommand(string FirstName, string LastName, string Email, string Password, string Role, int? CompanyId) : IRequest<Guid>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly UserManager<AppUser> _userManager;
    public CreateUserCommandHandler(UserManager<AppUser> userManager) => _userManager = userManager;

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CompanyId = request.CompanyId,
            RegisteredAt = DateTime.UtcNow
        };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Kullanıcı oluşturulamadı: {errors}");
        }

        await _userManager.AddToRoleAsync(user, request.Role);
        return user.Id;
    }
}
