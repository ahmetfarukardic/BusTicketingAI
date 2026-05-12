namespace BusTicketingAI.Domain.Entity;

public class Bus
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public int SeatCapacity { get; set; }

    public BusCompany Company { get; set; } = null!;
}
