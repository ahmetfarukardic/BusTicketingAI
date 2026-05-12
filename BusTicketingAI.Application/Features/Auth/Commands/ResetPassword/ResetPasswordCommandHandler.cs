using BusTicketingAI.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace BusTicketingAI.Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<bool>;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
{
    private readonly UserManager<AppUser> _userManager;

    public ResetPasswordCommandHandler(UserManager<AppUser> userManager) => _userManager = userManager;

    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email) ?? throw new Exception("Geçersiz işlem: Kullanıcı bulunamadı.");

        var decodedTokenBytes = WebEncoders.Base64UrlDecode(request.Token);
        var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Şifre sıfırlama başarısız: {errors}");
        }

        return true;
    }
}
