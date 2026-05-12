using BusTicketingAI.Application.Features.Tickets.Queries.GetOccupiedSeats;
using BusTicketingAI.Domain.Entity;

namespace BusTicketingAI.Application.Interfaces;

public interface ITicketRepository : IGenericRepository<Ticket>
{
    Task<bool> IsSeatTakenAsync(Guid tripId, int seatNumber, CancellationToken cancellationToken);
    Task<List<OccupiedSeatDto>> GetOccupiedSeatNumberAsync(Guid tripId, CancellationToken cancellationToken);
    Task<bool> HasSoldTicketsAsync(Guid tripId, CancellationToken cancellationToken);
    Task<Ticket?> GetTicketWithTripAsync(Guid ticketId, CancellationToken cancellationToken);
    Task<List<Ticket>> GetTicketsByTripAndCompanyAsync(Guid tripId, int companyId, CancellationToken cancellationToken);
    Task<List<Ticket>> GetTicketsWithTripByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
