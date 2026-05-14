using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Domain.Entity;
using MediatR;

namespace BusTicketingAI.Application.Features.WalletTransactions.Commands;

public record AdjustWalletBalanceCommand(Guid UserId, decimal Amount) : IRequest<bool>;

public class AdjustWalletBalanceCommandHandler : IRequestHandler<AdjustWalletBalanceCommand, bool>
{

    private readonly IWalletTransactionRepository _walletTransactionRepository;

    public AdjustWalletBalanceCommandHandler(IWalletTransactionRepository walletTransactionRepository) => _walletTransactionRepository = walletTransactionRepository;

    public async Task<bool> Handle(AdjustWalletBalanceCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount == 0)
            return true;

        var transaction = new WalletTransaction
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Amount = request.Amount,
            TransactionType = Domain.Enum.WalletTransactionType.AdminBonus,
            CreatedAt = DateTime.UtcNow
        };

        await _walletTransactionRepository.AddAsync(transaction, cancellationToken);
        await _walletTransactionRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
