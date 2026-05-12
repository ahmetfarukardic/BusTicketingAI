namespace BusTicketingAI.Application.Features.Trips;

public class CreateTripRequest
{
    public int BusId { get; set; }
    public int OriginTerminalId { get; set; }
    public int DestinationTerminalId { get; set; }
    public DateTime DepartureTime { get; set; }
    public decimal BasePrice { get; set; }
    public int EstimatedDuration { get; set; }
}

public class UpdateTripTimeRequest
{
    public DateTime NewDepartureTime { get; set; }
}