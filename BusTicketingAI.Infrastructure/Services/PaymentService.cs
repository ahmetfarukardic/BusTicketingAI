using BusTicketingAI.Application.DTOs;
using BusTicketingAI.Application.Interfaces;

namespace BusTicketingAI.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request, CancellationToken cancellationToken)
    {
        await Task.Delay(1500, cancellationToken);

        var sanitizedCardNumber = request.CardNumber.Replace(" ", "").Trim();

        if (sanitizedCardNumber == "1111111111111111")
        {
            return new PaymentResult(true, string.Empty, Guid.NewGuid().ToString());
        }
        return new PaymentResult(false, "Ödeme işlemi reddedildi: Lütfen kart numaranızı ve bakiyenizi kontrol edin.", string.Empty);
    }
}
