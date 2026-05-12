using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.Trips.Commands.UpdateTripTime;

public record UpdateTripTimeCommand(Guid TripId, DateTime NewDepartureDateTime, int CompanyId, CancellationToken CancellationToken) : IRequest<bool>;

public class UpdateTripTimeCommandHandler : IRequestHandler<UpdateTripTimeCommand, bool>
{
    private readonly ITripRepository _tripRepository;
    private readonly ITicketRepository _ticketRepository;

    public UpdateTripTimeCommandHandler(ITripRepository tripRepository, ITicketRepository ticketRepository)
    {
        _tripRepository = tripRepository;
        _ticketRepository = ticketRepository;
    }

    public async Task<bool> Handle(UpdateTripTimeCommand request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetTripWithBusAsync(request.TripId, cancellationToken);

        if (trip == null || trip.Bus?.CompanyId != request.CompanyId)
        {
            throw new UnauthorizedAccessException("Bu sefere müdahale etme yetkiniz yok.");
        }

        bool hasSoldTickets = await _ticketRepository.HasSoldTicketsAsync(request.TripId, cancellationToken);
        if (hasSoldTickets)
        {
            throw new InvalidOperationException("Bu sefere bilet satıldığı için saat değişikliği yapılamaz. Lütfen önce yolcularla iletişime geçip biletleri iptal edin.");
        }

        trip.DepartureTime = request.NewDepartureDateTime;
        await _tripRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
