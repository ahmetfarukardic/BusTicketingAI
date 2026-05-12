using BusTicketingAI.Application.Features.Tickets.Queries.GetOccupiedSeats;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace BusTicketingAI.Application.Features.Tickets.Commands.LockSeats;

public record UnlockSeatsCommand(Guid TripId, List<OccupiedSeatDto> Seats) : IRequest<bool>;

public class UnlockSeatsCommandHandler : IRequestHandler<UnlockSeatsCommand, bool>
{
    private readonly IMemoryCache _memoryCache;

    public UnlockSeatsCommandHandler(IMemoryCache memoryCache) => _memoryCache = memoryCache;

    public Task<bool> Handle(UnlockSeatsCommand request, CancellationToken cancellationToken)
    {
        var cacheKey = $"Trip_{request.TripId}_Locks";

        if (_memoryCache.TryGetValue(cacheKey, out List<OccupiedSeatDto>? lockedSeats) && lockedSeats != null)
        {
            lockedSeats.RemoveAll(ls => request.Seats.Any(rs => rs.SeatNumber == ls.SeatNumber));
            _memoryCache.Set(cacheKey, lockedSeats, TimeSpan.FromMinutes(5));
        }

        return Task.FromResult(true);
    }
}
