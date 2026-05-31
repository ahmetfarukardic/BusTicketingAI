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

        var availableSeats = new List<AvailableSeatDto>();
        bool is2Plus1 = trip.Bus.SeatCapacity == 30;

        for (int i = 1; i <= trip.Bus.SeatCapacity; i++)
        {
            if (!occupiedSeats.Any(s => s.SeatNumber == i))
            {
                string description = "";

                if (is2Plus1)
                {
                    if (i % 3 == 1) description = "Tekli Koltuk - Cam Kenarı";
                    else if (i % 3 == 2) description = "Çiftli Koltuk - Koridor";
                    else if (i % 3 == 0) description = "Çiftli Koltuk - Cam Kenarı";
                }
                else
                {
                    if (i % 4 == 1) description = "Sol Çiftli - Cam Kenarı";
                    else if (i % 4 == 2) description = "Sol Çiftli - Koridor";
                    else if (i % 4 == 3) description = "Sağ Çiftli - Koridor";
                    else if (i % 4 == 0) description = "Sağ Çiftli - Cam Kenarı";
                }

                availableSeats.Add(new AvailableSeatDto(i, description));
            }
        }

        return new TripDetailsResponseDto(
            trip.BasePrice,
            trip.Bus.SeatCapacity,
            occupiedSeats,
            availableSeats
        );
    }
}
