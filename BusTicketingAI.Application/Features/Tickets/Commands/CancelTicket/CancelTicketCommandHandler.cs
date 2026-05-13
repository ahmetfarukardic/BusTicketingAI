using BusTicketingAI.Application.Events;
using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.Tickets.Commands.CancelTicket;

public record CancelTicketCommand(Guid TicketId, int CompanyId) : IRequest<bool>;

public class CancelTicketCommandHandler : IRequestHandler<CancelTicketCommand, bool>
{
    private readonly ITicketRepository _ticketRepository;

    public CancelTicketCommandHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<bool> Handle(CancelTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetTicketWithTripAsync(request.TicketId, cancellationToken) ?? throw new Exception("Bilet bulunamadı.");

        if (ticket.Trip?.Bus?.CompanyId != request.CompanyId)
            throw new UnauthorizedAccessException("Bu bileti iptal etme yetkiniz yok.");

        if (ticket.Status == 0)
            throw new InvalidOperationException("Bu bilet zaten iptal edilmiş");

        ticket.Status = 0;

        if (ticket.User != null)
        {
            ticket.AddDomainEvent(new TicketCancelledEvent(
                ticket.Id,
                ticket.User.Email,
                ticket.PassengerName,
                "PNR" + ticket.Id.ToString()[..6].ToUpper(),
                ticket.Trip.OriginTerminal.Name,
                ticket.Trip.DestinationTerminal.Name,
                ticket.Trip.DepartureTime,
                ticket.SeatNumber,
                ticket.Trip.Bus.Company.Name
            ));
        }

        _ticketRepository.Update(ticket);
        await _ticketRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
