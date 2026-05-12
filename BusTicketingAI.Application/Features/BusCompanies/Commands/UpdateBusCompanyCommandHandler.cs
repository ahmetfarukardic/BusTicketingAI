using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.BusCompanies.Commands;

public record UpdateBusCompanyCommand(int Id, string Name) : IRequest;

public class UpdateBusCompanyCommandHandler : IRequestHandler<UpdateBusCompanyCommand>
{
    private readonly IBusCompanyRepository _busCompanyRepository;
    public UpdateBusCompanyCommandHandler(IBusCompanyRepository busCompanyRepository) => _busCompanyRepository = busCompanyRepository;

    public async Task Handle(UpdateBusCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _busCompanyRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new Exception("Guncellenecek firma bulunamadi");
        company.Name = request.Name;
        _busCompanyRepository.Update(company);
        await _busCompanyRepository.SaveChangesAsync(cancellationToken);
    }
}
