using BusTicketingAI.Domain.Entity;

namespace BusTicketingAI.Application.Interfaces;

public interface ITerminalRepository : IGenericRepository<Terminal>
{
    Task<List<Terminal>> GetAllTerminalsWithCitiesAsync(CancellationToken cancellationToken);
}
