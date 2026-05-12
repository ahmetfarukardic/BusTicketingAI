using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.Trips.Queries.GetCompanyActiveTrips;

public record GetCompanyActiveTripsQuery(int CompanyId, int? OriginalTerminalId, int? DestinationTerminalId, DateTime? DepartureDate) : IRequest<List<CompanyActiveTripDto>>;

public class GetCompanyActiveTripsQueryHandler : IRequestHandler<GetCompanyActiveTripsQuery, List<CompanyActiveTripDto>>
{
    private readonly ITripRepository _tripRepository;

    public GetCompanyActiveTripsQueryHandler(ITripRepository tripRepository) => _tripRepository = tripRepository;

    public async Task<List<CompanyActiveTripDto>> Handle(GetCompanyActiveTripsQuery request, CancellationToken cancellationToken)
    {
        var trips = await _tripRepository.GetActiveTripsByCompanyIdAsync(request.CompanyId, request.OriginalTerminalId, request.DestinationTerminalId, request.DepartureDate, cancellationToken);

        return trips.Select(t => new CompanyActiveTripDto
        {
            TripId = t.Id,
            OriginTerminal = t.OriginTerminal.Name,
            DestinationTerminal = t.DestinationTerminal.Name,
            DepartureTime = t.DepartureTime,
            BusPlate = t.Bus.PlateNumber,
            Price = t.BasePrice,
            TotalSeats = t.Bus.SeatCapacity
        }).ToList();
    }
}
