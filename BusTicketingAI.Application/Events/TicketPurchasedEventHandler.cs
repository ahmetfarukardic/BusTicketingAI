using BusTicketingAI.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusTicketingAI.Application.Events;

public class TicketPurchasedEventHandler : INotificationHandler<TicketPurchasedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<TicketPurchasedEventHandler> _logger;

    public TicketPurchasedEventHandler(IEmailService emailService, ILogger<TicketPurchasedEventHandler> logger)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(TicketPurchasedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            string subject = $"Biletiniz Onaylandı - PNR: {notification.PnrCode}";

            string htmlBody = $@"
            <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: auto; border: 1px solid #e5e7eb; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.05);'>
                <div style='background: linear-gradient(135deg, #1e3a8a, #3b82f6); color: white; padding: 30px 20px; text-align: center;'>
                    <h2 style='margin: 0; font-size: 24px;'>İyi Yolculuklar, {notification.PassengerName}!</h2>
                    <p style='margin: 10px 0 0 0; opacity: 0.9;'>Bilet işleminiz başarıyla tamamlandı.</p>
                </div>
                <div style='padding: 30px; background-color: #f8fafc;'>
                    <div style='background-color: white; padding: 25px; border-radius: 10px; border-left: 5px solid #10b981; box-shadow: 0 2px 4px rgba(0,0,0,0.02);'>
                        <h3 style='margin-top: 0; color: #1e293b; border-bottom: 1px solid #e2e8f0; padding-bottom: 10px;'>Sefer Detayları</h3>
                    
                        <p style='color: #475569; margin: 10px 0;'><strong>Otobüs Firması:</strong> {notification.CompanyName}</p>
                        <p style='color: #475569; margin: 10px 0;'><strong>Rota:</strong> {notification.OriginTerminal} ➔ {notification.DestinationTerminal}</p>
                        <p style='color: #475569; margin: 10px 0;'><strong>Kalkış Zamanı:</strong> {notification.DepartureTime:dd.MM.yyyy - HH:mm}</p>
                        <p style='color: #475569; margin: 10px 0;'><strong>Koltuk Numarası:</strong> <span style='background-color: #eff6ff; color: #2563eb; padding: 3px 8px; border-radius: 4px; font-weight: bold;'>{notification.SeatNumber}</span></p>
                        <p style='color: #475569; margin: 10px 0;'><strong>Ödenen Tutar:</strong> {notification.Price} ₺</p>
                    
                        <div style='margin-top: 25px; padding-top: 15px; border-top: 1px dashed #cbd5e1; text-align: center;'>
                            <p style='margin: 0; font-size: 14px; color: #64748b;'>PNR KODUNUZ</p>
                            <p style='margin: 5px 0 0 0; font-size: 28px; font-weight: 900; color: #0f172a; letter-spacing: 2px;'>{notification.PnrCode}</p>
                        </div>
                    </div>
                    <p style='margin-top: 25px; font-size: 13px; color: #94a3b8; text-align: center;'>Lütfen sefer saatinden en az 15 dakika önce peronda hazır bulununuz. Bizi tercih ettiğiniz için teşekkür ederiz.</p>
                </div>
            </div>";

            await _emailService.SendEmailAsync(notification.PassengerEmail, subject, htmlBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bilet satın alındı ancak PNR: {PnrCode} için bilgilendirme maili gönderilirken bir hata oluştu.", notification.PnrCode);
        }
    }
}