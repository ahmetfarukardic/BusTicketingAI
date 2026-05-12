using BusTicketingAI.Application.Events;
using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Domain.Entity;
using MediatR;

namespace BusTicketingAI.Application.Features.Tickets.Commands.SellTicket;

public record SellTicketCommand(Guid TripId, int SeatNumber, string PassengerName, string PassengerTC, decimal Price, int CompanyId) : IRequest<Guid>;

public class SellTicketCommandHandler : IRequestHandler<SellTicketCommand, Guid>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IMediator _mediator;

    public SellTicketCommandHandler(ITicketRepository ticketRepository, ITripRepository tripRepository, IMediator mediator)
    {
        _ticketRepository = ticketRepository;
        _tripRepository = tripRepository;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(SellTicketCommand request, CancellationToken cancellationToken)
    {
        var isAuthorized = await _tripRepository.IsTripBelongsToCompanyAsync(request.TripId, request.CompanyId, cancellationToken);
        if (!isAuthorized)
            throw new Exception("Yetkisiz işlem! Bu sefere bilet kesme yetkiniz bulunmuyor.");

        var isSeatTaken = await _ticketRepository.IsSeatTakenAsync(request.TripId, request.SeatNumber, cancellationToken);
        if(isSeatTaken)
            throw new Exception("Hata: Seçili koltuk daha önce satılmış!");

        var newTicket = new Ticket
        {
            Id = Guid.NewGuid(),
            TripId = request.TripId,
            SeatNumber = request.SeatNumber,
            PassengerName = request.PassengerName,
            PassengerTC = request.PassengerTC,
            Price = request.Price,
            Status = 1,
            CreatedAt = DateTime.UtcNow
        };

        await _ticketRepository.AddAsync(newTicket, cancellationToken);
        await _ticketRepository.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new TicketPurchasedEvent(
            newTicket.Id,
            newTicket.User.Email, 
            newTicket.PassengerName,
            "PNR" + newTicket.Id.ToString().Substring(0, 6).ToUpper(), 
            newTicket.Trip.OriginTerminal.Name,
            newTicket.Trip.DestinationTerminal.Name,
            newTicket.Trip.DepartureTime,
            newTicket.SeatNumber,
            newTicket.Price,
            newTicket.Trip.Bus.Company.Name
        ), cancellationToken);

        return newTicket.Id;
    }
}
