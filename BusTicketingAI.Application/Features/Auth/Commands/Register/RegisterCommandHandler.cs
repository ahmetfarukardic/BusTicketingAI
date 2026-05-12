using BusTicketingAI.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BusTicketingAI.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly UserManager<AppUser> _userManager;

    public RegisterCommandHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var userExist = await _userManager.FindByEmailAsync(request.Email);
        if (userExist != null)
            return new AuthResponseDto { Message = "Bu Email zaten kullaniliyor." };
        var user = new AppUser
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return new AuthResponseDto { Message = "Kullanici olusturulamadi. Sifreyi kontrol ediniz." };

        await _userManager.AddToRoleAsync(user, "Member");

        return new AuthResponseDto { Message = "Kayit basarili. Lutfen giris yapiniz." };
    }
}
