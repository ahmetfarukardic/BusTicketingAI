using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.BusCompanies.Queries;

public record GetAllBusCompaniesQuery : IRequest<List<BusCompanyResponseDto>>;

public class GetAllBusCompaniesQueryHandler : IRequestHandler<GetAllBusCompaniesQuery, List<BusCompanyResponseDto>>
{
    private readonly IBusCompanyRepository _busCompanyRepository;

    public GetAllBusCompaniesQueryHandler(IBusCompanyRepository busCompanyRepository) => _busCompanyRepository = busCompanyRepository;

    public async Task<List<BusCompanyResponseDto>> Handle(GetAllBusCompaniesQuery request, CancellationToken cancellationToken)
    {
        var companies = await _busCompanyRepository.GetAllWithBusesAsync(cancellationToken);
        return companies.Select(c => new BusCompanyResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            TotalBuses = c.Buses.Count
        }).ToList();
    }
}
