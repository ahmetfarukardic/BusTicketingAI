namespace BusTicketingAI.Domain.Entity;

public class Trip
{
    public Guid Id { get; set; }
    public int BusId { get; set; }
    public int OriginTerminalId { get; set; }
    public int DestinationTerminalId { get; set; }

    public DateTime DepartureTime { get; set; }
    public int EstimatedDuration { get; set; }
    public decimal BasePrice { get; set; }

    public Bus Bus { get; set; } = null!;
    public Terminal OriginTerminal { get; set; } = null!;
    public Terminal DestinationTerminal { get; set; } = null!;
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public int Status { get; set; } = 1;
}
