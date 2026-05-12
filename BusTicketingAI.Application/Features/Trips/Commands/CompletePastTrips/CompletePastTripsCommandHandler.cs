using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.Trips.Commands.CompletePastActiveTrips;

public record CompletePastTripsCommand() : IRequest<int>;

public class CompletePastTripsCommandHandler : IRequestHandler<CompletePastTripsCommand, int>
{
    private readonly ITripRepository _tripRepository;

    public CompletePastTripsCommandHandler(ITripRepository tripRepository) => _tripRepository = tripRepository;

    public async Task<int> Handle(CompletePastTripsCommand request, CancellationToken cancellationToken)
    {
        var currentTime = DateTime.UtcNow;
        var pastTrips = await _tripRepository.GetPastActiveTripsAsync(currentTime, cancellationToken);
        if (!pastTrips.Any())
            return 0;

        foreach ( var trip in pastTrips )
        {
            trip.Status = 2;
            _tripRepository.Update(trip);
        }
        await _tripRepository.SaveChangesAsync(cancellationToken);
        return pastTrips.Count;
    }
}
