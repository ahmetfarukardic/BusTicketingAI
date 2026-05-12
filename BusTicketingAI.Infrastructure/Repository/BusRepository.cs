using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Domain.Entity;
using BusTicketingAI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BusTicketingAI.Infrastructure.Repository;

public class BusRepository : GenericRepository<Bus>, IBusRepository
{
    public BusRepository(AppDbContext context) : base(context) { } 

    public async Task<List<Bus>> GetBusesByCompanyIdAsync(int companyId, CancellationToken cancellationToken)
    {
        return await _context.Buses
            .Where(b => b.CompanyId == companyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsPlateNumberExistsAsync(string plateNumber, CancellationToken cancellationToken)
    {
        return await _context.Buses.AnyAsync(b => b.PlateNumber == plateNumber, cancellationToken);
    }
}
