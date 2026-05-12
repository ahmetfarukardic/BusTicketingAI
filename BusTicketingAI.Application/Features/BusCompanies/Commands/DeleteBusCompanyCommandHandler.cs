using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.BusCompanies.Commands;

public record DeleteBusCompanyCommand(int Id) : IRequest;

public class DeleteBusCompanyCommandHandler : IRequestHandler<DeleteBusCompanyCommand>
{
    private readonly IBusCompanyRepository _busCompanyRepository;
    public DeleteBusCompanyCommandHandler(IBusCompanyRepository busCompanyRepository) => _busCompanyRepository = busCompanyRepository;

    public async Task Handle(DeleteBusCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _busCompanyRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new Exception("Silinecek firma bulunamadi");
        _busCompanyRepository.Delete(company);
        await _busCompanyRepository.SaveChangesAsync(cancellationToken);
    }
}
