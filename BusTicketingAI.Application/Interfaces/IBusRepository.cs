using BusTicketingAI.Domain.Entity;

namespace BusTicketingAI.Application.Interfaces;

public interface IBusRepository : IGenericRepository<Bus>
{
    Task<List<Bus>> GetBusesByCompanyIdAsync(int companyId, CancellationToken cancellationToken);
    Task<bool> IsPlateNumberExistsAsync(string plateNumber, CancellationToken cancellationToken);
}
