using MediatR;

namespace BusTicketingAI.Application.Features.Trips.Queries.SearchTrips;

public class SearchTripsQuery : IRequest<List<TripSearchResponseDto>>
{
    public int? OriginCityId { get; set; }
    public int? DestinationCityId { get; set; }
    public DateTime? DepartureDate { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
