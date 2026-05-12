using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.Buses.Commands.DeleteBus;

public record DeleteBusCommand(int BusId, int CompanyId) : IRequest<bool>;

public class DeleteBusCommandHandler : IRequestHandler<DeleteBusCommand, bool>
{
    private readonly IBusRepository _busRepository;
    private readonly ITripRepository _tripRepository;

    public DeleteBusCommandHandler(IBusRepository busRepository, ITripRepository tripRepository)
    {
        _busRepository = busRepository;
        _tripRepository = tripRepository;
    }

    public async Task<bool> Handle(DeleteBusCommand request, CancellationToken cancellationToken)
    {
        var bus = await _busRepository.GetByIdAsync(request.BusId, cancellationToken);
        if (bus == null || request.CompanyId != bus.CompanyId)
        {
            throw new UnauthorizedAccessException("Bu otobüsü silme yetkiniz yok veya otobüs bulunamadı.");
        }
        var hasUpcomingTrips = await _tripRepository.HasUpcomingTripsByBusIdAsync(request.BusId, cancellationToken);
        if (hasUpcomingTrips)
            throw new InvalidOperationException("Bu otobüsün planlanmış gelecek seferleri bulunduğu için sistemden silinemez. Lütfen önce seferleri iptal edin veya otobüslerini değiştirin.");

        _busRepository.Delete(bus);
        await _busRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
