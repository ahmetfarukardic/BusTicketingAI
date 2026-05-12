using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.Tickets.Queries.GetTripPassengers;

public record GetTripPassengersQuery(Guid TripId, int CompanyId) : IRequest<List<PassengerResponseDto>>;

public class GetTripPassengersQueryHandler : IRequestHandler<GetTripPassengersQuery, List<PassengerResponseDto>>
{
    private readonly ITicketRepository _ticketRepository;

    public GetTripPassengersQueryHandler(ITicketRepository ticketRepository) => _ticketRepository = ticketRepository;

    public async Task<List<PassengerResponseDto>> Handle(GetTripPassengersQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _ticketRepository.GetTicketsByTripAndCompanyAsync(request.TripId, request.CompanyId, cancellationToken);

        return tickets.Select(t => new PassengerResponseDto(
            t.Id,
            t.PassengerName,
            string.IsNullOrWhiteSpace(t.PassengerTC) || t.PassengerTC.Length < 4
                ? "***"
                : string.Concat("***", t.PassengerTC.AsSpan(t.PassengerTC.Length - 4)),
            t.SeatNumber,
            t.Status
        )).ToList();
    }
}
