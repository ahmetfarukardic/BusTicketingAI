using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Domain.Entity;
using MediatR;

namespace BusTicketingAI.Application.Features.Trips.Commands.CreateTrip;

public record CreateTripCommand(
    int BusId, int OriginalTerminalId,
    int DestinationTerminalId,
    DateTime DepartureDate,
    decimal BasePrice,
    int EstimatedDuration,
    int CompanyId
    ) : IRequest<Guid>;

public class CreateTripCommandHandler : IRequestHandler<CreateTripCommand, Guid>
{
    private readonly ITripRepository _tripRepository;
    private readonly IBusRepository _busRepository;

    public CreateTripCommandHandler(ITripRepository tripRepository, IBusRepository busRepository)
    {
        _tripRepository = tripRepository;
        _busRepository = busRepository;
    }

    public async Task<Guid> Handle(CreateTripCommand request, CancellationToken cancellationToken)
    {
        var bus = await _busRepository.GetByIdAsync(request.BusId, cancellationToken);

        if (bus == null || bus.CompanyId !=  request.CompanyId)
        {
            throw new UnauthorizedAccessException("Bu otobüs firmanıza ait değil veya bulunamadı!");
        }

        var newTrip = new Trip
        {
            Id = Guid.NewGuid(),
            BusId = request.BusId,
            OriginTerminalId = request.OriginalTerminalId,
            DestinationTerminalId = request.DestinationTerminalId,
            DepartureTime = request.DepartureDate,
            BasePrice = request.BasePrice,
            EstimatedDuration = request.EstimatedDuration,
            Status = 1
        };

        await _tripRepository.AddAsync(newTrip, cancellationToken);
        await _tripRepository.SaveChangesAsync(cancellationToken);

        return newTrip.Id;
    }
}
