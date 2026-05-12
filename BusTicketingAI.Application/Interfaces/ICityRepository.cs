using BusTicketingAI.Application.Features.Cities.Queries.GetAllCities;
using BusTicketingAI.Domain.Entity;

namespace BusTicketingAI.Application.Interfaces;

public interface ICityRepository : IGenericRepository<City>
{
    Task<List<City>> GetAllCitiesOrderedAsync(CancellationToken cancellationToken);
}
