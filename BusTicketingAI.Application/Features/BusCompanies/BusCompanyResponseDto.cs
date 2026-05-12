namespace BusTicketingAI.Application.Features.BusCompanies;

public class BusCompanyResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalBuses { get; set; }
}
