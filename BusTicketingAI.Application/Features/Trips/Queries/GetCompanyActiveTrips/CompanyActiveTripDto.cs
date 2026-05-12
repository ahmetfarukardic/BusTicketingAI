namespace BusTicketingAI.Application.Features.Trips.Queries.GetCompanyActiveTrips;

public class CompanyActiveTripDto
{
    public Guid TripId { get; set; }
    public string OriginTerminal { get; set; } = string.Empty;
    public string DestinationTerminal { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public string BusPlate { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int TotalSeats { get; set; }
}
