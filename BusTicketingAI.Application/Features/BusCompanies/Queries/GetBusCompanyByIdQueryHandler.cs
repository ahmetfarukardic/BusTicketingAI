using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.BusCompanies.Queries;

public record GetBusCompanyByIdQuery(int Id) : IRequest<BusCompanyResponseDto?>;

public class GetBusCompanyByIdQueryHandler : IRequestHandler<GetBusCompanyByIdQuery, BusCompanyResponseDto?>
{
    private readonly IBusCompanyRepository _busCompanyRepository;
    public GetBusCompanyByIdQueryHandler(IBusCompanyRepository busCompanyRepository) => _busCompanyRepository = busCompanyRepository;

    public async Task<BusCompanyResponseDto?> Handle(GetBusCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await _busCompanyRepository.GetByIdAsync(request.Id, cancellationToken);

        if (company == null) 
            return null;

        return new BusCompanyResponseDto
        {
            Id = company.Id,
            Name = company.Name,
            TotalBuses = company.Buses?.Count ?? 0
        };
    }
}
