namespace BusTicketingAI.Application.DTOs;

public record PaymentRequest(string CardHolderName, string CardNumber, string ExpirationDate, string Cvv, decimal TotalAmount);

public record PaymentResult(bool IsSuccessful, string ErrorMessage, string TransactionId);