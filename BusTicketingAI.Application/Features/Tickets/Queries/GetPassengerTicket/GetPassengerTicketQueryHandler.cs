using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.Tickets.Queries.GetPassengerTicket;

public record GetPassengerTicketQuery(Guid UserID) : IRequest<PassengerTicketResponseDto>;

public class GetPassengerTicketQueryHandler : IRequestHandler<GetPassengerTicketQuery, PassengerTicketResponseDto>
{
    private readonly ITicketRepository _ticketRepository;

    public GetPassengerTicketQueryHandler(ITicketRepository ticketRepository) => _ticketRepository = ticketRepository;

    public async Task<PassengerTicketResponseDto> Handle(GetPassengerTicketQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _ticketRepository.GetTicketsWithTripByUserIdAsync(request.UserID, cancellationToken);

        var ticketDtos = tickets.Select(t => new TicketDetailDto(
            TicketId: t.Id,
            PassengerName: t.PassengerName,
            SeatNumber: t.SeatNumber,
            Price: t.Price,
            Status: t.Status,
            DepartureTerminal: t.Trip?.OriginTerminal.Name ?? "Bilinmiyor",
            ArrivalTerminal: t.Trip?.DestinationTerminal?.Name ?? "Bilinmiyor",
            DepartureTime: t.Trip?.DepartureTime ?? DateTime.MinValue
        )).ToList();

        var now = DateTime.UtcNow;
        var activeTickets = ticketDtos
            .Where(t => t.Status == 1 && t.DepartureTime > now)
            .OrderBy(t => t.DepartureTime)
            .ToList();
        var pastTickets = ticketDtos
            .Where(t => t.Status == 0 || t.DepartureTime <= now)
            .OrderBy(t => t.DepartureTime)
            .ToList();

        return new PassengerTicketResponseDto(activeTickets, pastTickets);
    }
}
