using BusTicketingAI.Application.Events;
using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Domain.Entity;
using MediatR;

namespace BusTicketingAI.Application.Features.Tickets.Commands.SellTicket;

public record SellTicketCommand(Guid TripId, int SeatNumber, string PassengerName, string PassengerTC, decimal Price, string Gender, int CompanyId) : IRequest<Guid>;

public class SellTicketCommandHandler : IRequestHandler<SellTicketCommand, Guid>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ITripRepository _tripRepository;

    public SellTicketCommandHandler(ITicketRepository ticketRepository, ITripRepository tripRepository)
    {
        _ticketRepository = ticketRepository;
        _tripRepository = tripRepository;
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
            Gender = request.Gender,
            Status = 1,
            CreatedAt = DateTime.UtcNow
        };

        await _ticketRepository.AddAsync(newTicket, cancellationToken);
        await _ticketRepository.SaveChangesAsync(cancellationToken);
        return newTicket.Id;
    }
}
