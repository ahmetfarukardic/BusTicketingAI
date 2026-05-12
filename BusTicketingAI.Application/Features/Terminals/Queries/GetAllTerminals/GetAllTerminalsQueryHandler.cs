using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.Terminals.Queries.GetAllTerminals;

public record GetAllTerminalsQuery : IRequest<List<TerminalResponseDto>>;

public class GetAllTerminalsQueryHandler : IRequestHandler<GetAllTerminalsQuery, List<TerminalResponseDto>>
{
    private readonly ITerminalRepository _terminalRepository;

    public GetAllTerminalsQueryHandler(ITerminalRepository terminalRepository) => _terminalRepository = terminalRepository;

    public async Task<List<TerminalResponseDto>> Handle(GetAllTerminalsQuery request, CancellationToken cancellationToken)
    {
        var terminals = await _terminalRepository.GetAllTerminalsWithCitiesAsync(cancellationToken);
        return terminals.Select(t => new TerminalResponseDto
        {
            Id = t.Id,
            Name = t.Name,
            CityName = t.City != null ? t.City.Name : ""
        })
        .OrderBy(t => t.CityName).ThenBy(t => t.Name)
        .ToList();
    }
}
