using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Domain.Entity;
using MediatR;

namespace BusTicketingAI.Application.Features.BusCompanies.Commands;

public record CreateBusCompanyCommand(string Name) : IRequest<int>;

public class CreateBusCompanyCommandHandler : IRequestHandler<CreateBusCompanyCommand, int>
{
    private readonly IBusCompanyRepository _busCompanyRepository;

    public CreateBusCompanyCommandHandler(IBusCompanyRepository busCompanyRepository) => _busCompanyRepository = busCompanyRepository;

    public async Task<int> Handle(CreateBusCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = new BusCompany { Name = request.Name };
        await _busCompanyRepository.AddAsync(company, cancellationToken);
        await _busCompanyRepository.SaveChangesAsync(cancellationToken);
        return company.Id;
    }
}
