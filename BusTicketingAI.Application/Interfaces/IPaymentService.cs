using BusTicketingAI.Application.DTOs;

namespace BusTicketingAI.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request, CancellationToken cancellationToken);
}
