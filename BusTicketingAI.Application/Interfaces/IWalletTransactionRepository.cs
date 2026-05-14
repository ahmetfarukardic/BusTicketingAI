using BusTicketingAI.Domain.Entity;

namespace BusTicketingAI.Application.Interfaces;

public interface IWalletTransactionRepository : IGenericRepository<WalletTransaction>
{
    Task<decimal> GetBalanceByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<WalletTransaction>> GetTransactionsByUserId(Guid userId, CancellationToken cancellationToken);
}
