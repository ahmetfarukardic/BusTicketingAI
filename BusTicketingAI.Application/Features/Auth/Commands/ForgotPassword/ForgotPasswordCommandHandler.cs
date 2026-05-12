using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace BusTicketingAI.Application.Features.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<bool>;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(UserManager<AppUser> userManager, IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email) ?? throw new Exception("Kayıtlı e-posta adresi bulunamadı.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        string resetLink = $"http://localhost:4200/reset-password?token={encodedToken}&email={user.Email}";

        string subject = "BusTicketingAI - Şifre Sıfırlama Talebi";

        string htmlBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #e2e8f0; border-radius: 10px;'>
                <h2 style='color: #3b82f6;'>Şifre Sıfırlama Talebi</h2>
                <p>Merhaba {user.FirstName},</p>
                <p>Hesabınızın şifresini sıfırlamak için bir talepte bulundunuz. İşleme devam etmek için aşağıdaki butona tıklayın:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <a href='{resetLink}' style='background-color: #10b981; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Şifremi Sıfırla</a>
                </div>
                <p style='color: #ef4444; font-size: 0.9rem;'>Bu bağlantı güvenliğiniz için kısa bir süre sonra geçersiz olacaktır.</p>
            </div>";

        await _emailService.SendEmailAsync(request.Email, subject, htmlBody);

        return true;
    }
}
