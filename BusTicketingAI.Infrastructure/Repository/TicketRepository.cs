using BusTicketingAI.Application.Features.Tickets.Queries.GetOccupiedSeats;
using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Domain.Entity;
using BusTicketingAI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BusTicketingAI.Infrastructure.Repository;

public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
{
    public TicketRepository(AppDbContext context) : base(context) { }

    public async Task<bool> IsSeatTakenAsync(Guid tripId, int seatNumber, CancellationToken cancellationToken)
    {
        return await _context.Tickets.AnyAsync(t => t.TripId == tripId && t.SeatNumber == seatNumber && t.Status == 1, cancellationToken);
    }

    public async Task<List<OccupiedSeatDto>> GetOccupiedSeatNumberAsync(Guid tripId, CancellationToken cancellationToken)
    {
        return await _context.Tickets
            .Where(t => t.TripId == tripId && t.Status == 1)
            .Select(t => new OccupiedSeatDto(t.SeatNumber, t.Gender))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasSoldTicketsAsync(Guid tripId, CancellationToken cancellationToken)
    {
        return await _context.Tickets.AnyAsync(t => t.TripId == tripId && t.Status == 1, cancellationToken);
    }

    public async Task<Ticket?> GetTicketWithTripAsync(Guid ticketId, CancellationToken cancellationToken)
    {
        return await _context.Tickets.Include(t => t.Trip).ThenInclude(tr => tr.Bus).FirstOrDefaultAsync(t => t.Id == ticketId, cancellationToken);
    }

    public async Task<List<Ticket>> GetTicketsByTripAndCompanyAsync(Guid tripId, int companyId, CancellationToken cancellationToken)
    {
        return await _context.Tickets
            .Include(t => t.Trip).ThenInclude(tr => tr.Bus)
            .Where(t => t.TripId == tripId && t.Trip.Bus.CompanyId == companyId)
            .OrderBy(t => t.SeatNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Ticket>> GetTicketsWithTripByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Tickets
            .Include(t => t.Trip)
            .Include(t => t.Trip.DestinationTerminal)
            .Include(t => t.Trip.OriginTerminal)
            .Where(t => t.UserId == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
