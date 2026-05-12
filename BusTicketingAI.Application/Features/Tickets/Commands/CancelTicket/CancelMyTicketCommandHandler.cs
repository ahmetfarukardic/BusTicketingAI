using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.Tickets.Commands.CancelTicket;

public record CancelMyTicketCommand(Guid TicketId, Guid UserId) : IRequest<bool>;

public class CancelMyTicketCommandHandler : IRequestHandler<CancelMyTicketCommand, bool>
{
    private readonly ITicketRepository _ticketRepository;

    public CancelMyTicketCommandHandler(ITicketRepository ticketRepository) => _ticketRepository = ticketRepository;

    public async Task<bool> Handle(CancelMyTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId, cancellationToken) ?? throw new Exception("Bilet bulunamadı.");

        if (ticket.UserId != request.UserId)
            throw new UnauthorizedAccessException("Bu bileti iptal etme yetkiniz bulunmuyor.");

        if (ticket.Status == 0)
            throw new Exception("Bu bilet zaten iptal edilmiş.");

        //if (ticket.Trip.DepartureTime < DateTime.UtcNow) 
        //   throw new Exception("Geçmiş seferler iptal edilemez.");

        ticket.Status = 0;

        _ticketRepository.Update(ticket);
        await _ticketRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
