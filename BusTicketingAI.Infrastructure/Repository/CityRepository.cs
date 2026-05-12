using BusTicketingAI.Application.Features.Cities.Queries.GetAllCities;
using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Domain.Entity;
using BusTicketingAI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BusTicketingAI.Infrastructure.Repository;

public class CityRepository : GenericRepository<City>, ICityRepository
{
    public CityRepository(AppDbContext context) : base(context) { }

    public async Task<List<City>> GetAllCitiesOrderedAsync(CancellationToken cancellationToken)
    {
        return await _context.Cities
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
}
