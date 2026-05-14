using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Domain.Entity;
using BusTicketingAI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BusTicketingAI.Infrastructure.Repository;

public class WalletTransactionRepository : GenericRepository<WalletTransaction>, IWalletTransactionRepository
{
    public WalletTransactionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<decimal> GetBalanceByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.WalletTransactions
            .Where(w => w.UserId == userId)
            .SumAsync(w => w.Amount, cancellationToken);
    }

    public async Task<List<WalletTransaction>> GetTransactionsByUserId(Guid userId, CancellationToken cancellationToken)
    {
        
        return await _context.WalletTransactions
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
