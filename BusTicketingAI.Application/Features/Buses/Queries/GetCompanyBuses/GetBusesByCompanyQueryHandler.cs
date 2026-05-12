using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.Buses.Queries.GetCompanyBuses;

public record GetBusesByCompanyQuery(int CompanyId) : IRequest<List<BusResponseDto>>;

public class GetBusesByCompanyQueryHandler : IRequestHandler<GetBusesByCompanyQuery, List<BusResponseDto>>
{
    private readonly IBusRepository _busRepository;

    public GetBusesByCompanyQueryHandler(IBusRepository busRepository) => _busRepository = busRepository;
    
    public async Task<List<BusResponseDto>> Handle(GetBusesByCompanyQuery request, CancellationToken cancellationToken)
    {
        var buses = await _busRepository.GetBusesByCompanyIdAsync(request.CompanyId, cancellationToken);
        return buses.Select(b => new BusResponseDto(
            b.Id,
            b.PlateNumber,
            b.SeatCapacity,
            b.CompanyId
        )).ToList();
    }
}
