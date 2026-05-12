namespace BusTicketingAI.Domain.Entity;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Terminal> Terminals { get; set; } = new List<Terminal>();
}
