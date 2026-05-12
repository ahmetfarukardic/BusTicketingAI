using BusTicketingAI.Application.Features.Tickets.Queries.GetOccupiedSeats;
using BusTicketingAI.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace BusTicketingAI.Application.Features.Trips.Queries.GetTripDetails;

public record GetTripDetailsQuery(Guid TripId) : IRequest<TripDetailsResponseDto>;

public class GetTripDetailsQueryHandler : IRequestHandler<GetTripDetailsQuery, TripDetailsResponseDto>
{
    private readonly IMemoryCache _memoryCache;
    private readonly ITripRepository _tripRepository;
    private readonly ITicketRepository _ticketRepository;

    public GetTripDetailsQueryHandler(ITripRepository tripRepository, ITicketRepository ticketRepository, IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _tripRepository = tripRepository;
        _ticketRepository = ticketRepository;
    }

    public async Task<TripDetailsResponseDto> Handle(GetTripDetailsQuery request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetTripWithBusAsync(request.TripId, cancellationToken);
        if (trip == null || trip.Bus == null)
            throw new Exception("Sefer veya bağlı otobüs bulunamadı.");

        var occupiedSeats = await _ticketRepository.GetOccupiedSeatNumberAsync(request.TripId, cancellationToken);

        var cacheKey = $"Trip_{request.TripId}_Locks";
        if (_memoryCache.TryGetValue(cacheKey, out List<OccupiedSeatDto>? lockedSeats) && lockedSeats != null)
        {
            occupiedSeats.AddRange(lockedSeats);
            occupiedSeats = occupiedSeats.DistinctBy(s => s.SeatNumber).ToList();
        }

        return new TripDetailsResponseDto(
            trip.BasePrice,
            trip.Bus.SeatCapacity,
            occupiedSeats
        );
    }
}
