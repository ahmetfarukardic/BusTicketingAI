using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.WalletTransactions.Queries;

public record GetWalletTransactionQuery(Guid UserId) : IRequest<List<WalletTransactionDto>>;

public class GetWalletTransactionQueryHandler : IRequestHandler<GetWalletTransactionQuery, List<WalletTransactionDto>>
{
    private readonly IWalletTransactionRepository _walletTransactionRepository;

    public GetWalletTransactionQueryHandler(IWalletTransactionRepository walletTransactionRepository) => _walletTransactionRepository = walletTransactionRepository;

    public async Task<List<WalletTransactionDto>> Handle(GetWalletTransactionQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _walletTransactionRepository.GetTransactionsByUserId(request.UserId, cancellationToken);

        return transactions.Select(t => new WalletTransactionDto(
            t.Id,
            t.Amount,
            t.TransactionType.ToString(),
            t.ReferenceId,
            t.CreatedAt
        )).ToList();
    }
}