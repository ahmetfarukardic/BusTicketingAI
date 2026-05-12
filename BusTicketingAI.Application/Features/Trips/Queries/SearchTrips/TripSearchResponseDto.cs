namespace BusTicketingAI.Application.Features.Trips.Queries.SearchTrips;

public class TripSearchResponseDto
{
    public Guid TripId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string OriginTerminal { get; set; } = string.Empty;
    public string DestinationTerminal { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public decimal Price { get; set; }
    public int EmptySeats { get; set; }
    public int EstimatedDuration { get; set; }
}