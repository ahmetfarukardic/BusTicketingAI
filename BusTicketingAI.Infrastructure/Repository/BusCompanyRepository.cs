using BusTicketingAI.Application.DTOs;
using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Domain.Entity;
using BusTicketingAI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BusTicketingAI.Infrastructure.Repository;

public class BusCompanyRepository : GenericRepository<BusCompany>, IBusCompanyRepository
{
    public BusCompanyRepository(AppDbContext context) : base(context) { }

    public async Task<List<BusCompany>> GetAllWithBusesAsync(CancellationToken cancellationToken)
    {
        return await _context.BusCompanies
            .Include(c => c.Buses) 
            .ToListAsync(cancellationToken);
    }
}
