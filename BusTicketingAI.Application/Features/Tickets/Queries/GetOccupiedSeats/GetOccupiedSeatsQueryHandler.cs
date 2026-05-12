using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.Tickets.Queries.GetOccupiedSeats;

public record GetOccupiedSeatsQuery(Guid TripId) : IRequest<List<OccupiedSeatDto>>;

public class GetOccupiedSeatsQueryHandler : IRequestHandler<GetOccupiedSeatsQuery, List<OccupiedSeatDto>>
{
    private readonly ITicketRepository _ticketRepository;

    public GetOccupiedSeatsQueryHandler(ITicketRepository ticketRepository) => _ticketRepository = ticketRepository;

    public async Task<List<OccupiedSeatDto>> Handle(GetOccupiedSeatsQuery request, CancellationToken cancellationToken)
    {
        return await _ticketRepository.GetOccupiedSeatNumberAsync(request.TripId,cancellationToken);
    }
}
