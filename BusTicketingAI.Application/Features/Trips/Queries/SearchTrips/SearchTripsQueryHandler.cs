using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.Trips.Queries.SearchTrips;

public class SearchTripsQueryHandler : IRequestHandler<SearchTripsQuery, List<TripSearchResponseDto>>
{
    private readonly ITripRepository _tripRepository;

    public SearchTripsQueryHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<List<TripSearchResponseDto>> Handle(SearchTripsQuery request, CancellationToken cancellationToken)
    {
        var trips = await _tripRepository.SearchAndPaginateTripsAsync(
            request.OriginCityId,
            request.DestinationCityId,
            request.DepartureDate,
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        var result = trips.Select(trip => new TripSearchResponseDto
        {
            TripId = trip.Id,
            CompanyName = trip.Bus.Company.Name,
            OriginTerminal = trip.OriginTerminal.Name,
            DestinationTerminal = trip.DestinationTerminal.Name,
            DepartureTime = trip.DepartureTime,
            Price = trip.BasePrice,
            EmptySeats = trip.Bus.SeatCapacity - trip.Tickets.Count(ticket => ticket.Status == 1),
            EstimatedDuration = trip.EstimatedDuration
        }).ToList();

        return result;
    }
}
