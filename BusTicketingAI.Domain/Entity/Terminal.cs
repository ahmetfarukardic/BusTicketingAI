namespace BusTicketingAI.Domain.Entity;

public class Terminal
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public string Name { get; set; } = string.Empty;

    public City City { get; set; } = null!;
}
