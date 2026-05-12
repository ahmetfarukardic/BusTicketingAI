using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Domain.Entity;
using BusTicketingAI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BusTicketingAI.Infrastructure.Repository;

public class TerminalRepository : GenericRepository<Terminal>, ITerminalRepository
{
    public TerminalRepository(AppDbContext context) : base(context) { }

    public async Task<List<Terminal>> GetAllTerminalsWithCitiesAsync(CancellationToken cancellationToken)
    {
        return await _context.Terminals
            .Include(t => t.City)
            .ToListAsync(cancellationToken);
    }
}
