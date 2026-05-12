using BusTicketingAI.Application.Features.Tickets.Queries.GetOccupiedSeats;
using BusTicketingAI.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace BusTicketingAI.Application.Features.Tickets.Commands.LockSeats;

public record LockSeatsCommand(Guid TripId, List<OccupiedSeatDto> Seats) : IRequest<(bool IsSuccess, string Message)>;

public class LockSeatsCommandHandler : IRequestHandler<LockSeatsCommand, (bool IsSuccess, string Message)>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IMemoryCache _memoryCache;

    public LockSeatsCommandHandler(ITicketRepository ticketRepository, IMemoryCache memoryCache)
    {
        _ticketRepository = ticketRepository;
        _memoryCache = memoryCache;
    }

    public async Task<(bool IsSuccess, string Message)> Handle(LockSeatsCommand request, CancellationToken cancellationToken)
    {
        if (request.Seats == null || !request.Seats.Any())
            return (false, "Koltuk secimi backende ulasti");

        var cacheKey = $"Trip_{request.TripId}_Locks";

        if (!_memoryCache.TryGetValue(cacheKey, out List<OccupiedSeatDto>? lockedSeats) || lockedSeats == null)
        {
            lockedSeats = new List<OccupiedSeatDto>();
        }
        if (request.Seats.Any(s => s.SeatNumber <= 0))
            return (false, "Sistem koltuk numaralarını okuyamadı (JSON Binding Hatası). Lütfen modeli kontrol edin.");

        var conflictedSeats = lockedSeats.Where(ls => request.Seats.Any(rs => rs.SeatNumber == ls.SeatNumber)).ToList();
        if (conflictedSeats.Any())
        {
            var seatsStr = string.Join(", ", conflictedSeats.Select(x => x.SeatNumber));
            return (false, $"Seçtiğiniz {seatsStr} numaralı koltuklar şu an başka birisinin sepetinde.");
        }

        foreach (var seat in request.Seats)
        {
            if (await _ticketRepository.IsSeatTakenAsync(request.TripId, seat.SeatNumber, cancellationToken))
                return (false, $"{seat.SeatNumber} numaralı koltuk zaten satılmış.");
        }

        var newLockedList = new List<OccupiedSeatDto>(lockedSeats);
        newLockedList.AddRange(request.Seats);

        _memoryCache.Set(cacheKey, newLockedList, TimeSpan.FromMinutes(5));

        return (true, "Kilit başarılı.");
    }
}
