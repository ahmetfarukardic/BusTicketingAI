namespace BusTicketingAI.Application.Features.Terminals.Queries.GetAllTerminals;

public class TerminalResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
}
