namespace BusTicketingAI.Domain.Entity;

public class BusCompany
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Bus> Buses { get; set; } = new List<Bus>();
}
