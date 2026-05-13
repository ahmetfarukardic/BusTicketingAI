using BusTicketingAI.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusTicketingAI.Application.Events;

public class TicketCancelledEventHandler : INotificationHandler<TicketCancelledEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<TicketCancelledEventHandler> _logger;

    public TicketCancelledEventHandler(IEmailService emailService, ILogger<TicketCancelledEventHandler> logger)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(TicketCancelledEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            string subject = $"Bilet İptal Bilgilendirmesi - PNR: {notification.PnrCode}";

            string htmlBody = $@"
            <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: auto; border: 1px solid #e5e7eb; border-radius: 12px; overflow: hidden;'>
                <div style='background: linear-gradient(135deg, #991b1b, #ef4444); color: white; padding: 25px; text-align: center;'>
                    <h2 style='margin: 0;'>Biletiniz İptal Edilmiştir</h2>
                </div>
                <div style='padding: 30px; background-color: #f8fafc;'>
                    <p style='font-size: 16px; color: #334155;'>Sayın {notification.PassengerName},</p>
                    <p style='font-size: 15px; color: #475569; line-height: 1.5;'><strong>{notification.PnrCode}</strong> PNR kodlu, <strong>{notification.DepartureTime:dd.MM.yyyy HH:mm}</strong> tarihli <strong>{notification.OriginTerminal} - {notification.DestinationTerminal}</strong> seferine ait biletiniz başarıyla iptal edilmiştir.</p>
                
                    <div style='background-color: #fef2f2; border-left: 4px solid #ef4444; padding: 15px; margin-top: 20px; border-radius: 6px;'>
                        <p style='margin: 0; color: #991b1b; font-size: 14px;'>İptal işlemine ait ücret iadesi, bankanızın prosedürlerine bağlı olarak 1-3 iş günü içerisinde ödeme yaptığınız kartınıza yansıtılacaktır.</p>
                    </div>
                
                    <p style='margin-top: 30px; font-size: 14px; color: #64748b;'>Sizi tekrar aramızda görmek dileğiyle.<br><strong>{notification.CompanyName}</strong></p>
                </div>
            </div>";

            await _emailService.SendEmailAsync(notification.PassengerEmail, subject, htmlBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bilet iptal edildi ancak PNR: {PnrCode} için bilgilendirme maili gönderilirken bir hata oluştu.", notification.PnrCode);
        }
    }
}
