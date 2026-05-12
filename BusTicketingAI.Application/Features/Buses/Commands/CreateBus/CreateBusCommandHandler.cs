using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Domain.Entity;
using MediatR;

namespace BusTicketingAI.Application.Features.Buses.Commands.CreateBus;

public record CreateBusCommand(string PlateNumber, int SeatCapacity, int CompanyId) : IRequest<int>;

public class CreateBusCommandHandler : IRequestHandler<CreateBusCommand, int>
{
    private readonly IBusRepository _busRepository;

    public CreateBusCommandHandler(IBusRepository busRepository) => _busRepository = busRepository;
    
    public async Task<int> Handle(CreateBusCommand request, CancellationToken cancellationToken)
    {
        var exist = await _busRepository.IsPlateNumberExistsAsync(request.PlateNumber, cancellationToken);
        if (exist) throw new Exception("Bu plakaya ait bir otobüs zaten sistemde kayıtlı!");

        var newBus = new Bus
        {
            PlateNumber = request.PlateNumber,
            SeatCapacity = request.SeatCapacity,
            CompanyId = request.CompanyId
        };

        await _busRepository.AddAsync(newBus, cancellationToken);
        await _busRepository.SaveChangesAsync(cancellationToken);
        return newBus.Id;
    }
}
