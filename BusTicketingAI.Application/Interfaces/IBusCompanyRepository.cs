using BusTicketingAI.Domain.Entity;

namespace BusTicketingAI.Application.Interfaces;

public interface IBusCompanyRepository : IGenericRepository<BusCompany>
{
    Task<List<BusCompany>> GetAllWithBusesAsync(CancellationToken cancellationToken);
}
