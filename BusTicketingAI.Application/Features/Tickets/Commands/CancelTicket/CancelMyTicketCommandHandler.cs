using BusTicketingAI.Application.Events;
using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Domain.Entity;
using BusTicketingAI.Domain.Enum;
using MediatR;

namespace BusTicketingAI.Application.Features.Tickets.Commands.CancelTicket;

public record CancelMyTicketCommand(Guid TicketId, Guid UserId) : IRequest<bool>;

public class CancelMyTicketCommandHandler : IRequestHandler<CancelMyTicketCommand, bool>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IWalletTransactionRepository _walletTransactionRepository;

    public CancelMyTicketCommandHandler(ITicketRepository ticketRepository, IWalletTransactionRepository walletTransactionRepository)
    {
        _ticketRepository = ticketRepository;
        _walletTransactionRepository = walletTransactionRepository;
    }

    public async Task<bool> Handle(CancelMyTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetTicketWithTripAsync(request.TicketId, cancellationToken) ?? throw new Exception("Bilet bulunamadı.");

        if (ticket.UserId != request.UserId)
            throw new UnauthorizedAccessException("Bu bileti iptal etme yetkiniz bulunmuyor.");

        if (ticket.Status == 0)
            throw new Exception("Bu bilet zaten iptal edilmiş.");

        if (ticket.Trip.DepartureTime < DateTime.UtcNow) 
           throw new Exception("Geçmiş seferler iptal edilemez.");

        ticket.Status = 0;

        var refundTransaction = new WalletTransaction
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Amount = ticket.Price,
            TransactionType = WalletTransactionType.Refund,
            ReferenceId = ticket.Id,
            CreatedAt = DateTime.UtcNow,
        };

        await _walletTransactionRepository.AddAsync(refundTransaction, cancellationToken);

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