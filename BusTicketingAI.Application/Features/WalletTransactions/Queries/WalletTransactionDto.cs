namespace BusTicketingAI.Application.Features.WalletTransactions.Queries;

public record WalletTransactionDto(
    Guid Id,
    decimal Amount,
    string TransactionType,
    Guid? ReferenceId,
    DateTime CreatedAt
);