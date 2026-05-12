using BusTicketingAI.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace BusTicketingAI.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration) => _configuration = configuration;

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:SenderEmail"]));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = htmlBody };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:SmtpPort"]), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_configuration["EmailSettings:SenderEmail"], _configuration["EmailSettings:Password"]);
            await smtp.SendAsync(email);
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}
