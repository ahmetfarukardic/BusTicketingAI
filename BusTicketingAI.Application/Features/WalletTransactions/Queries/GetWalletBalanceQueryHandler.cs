using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.WalletTransactions.Queries;

public record GetWalletBalanceQuery(Guid UserId) : IRequest<decimal>;

public class GetWalletBalanceQueryHandler : IRequestHandler<GetWalletBalanceQuery, decimal>
{
    private readonly IWalletTransactionRepository _walletTransactionRepository;

    public GetWalletBalanceQueryHandler(IWalletTransactionRepository walletTransactionRepository) => _walletTransactionRepository = walletTransactionRepository;

    public async Task<decimal> Handle(GetWalletBalanceQuery request, CancellationToken cancellationToken)
    {
        return await _walletTransactionRepository.GetBalanceByUserIdAsync(request.UserId, cancellationToken);
    }
}