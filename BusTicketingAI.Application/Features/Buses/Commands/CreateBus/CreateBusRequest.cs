namespace BusTicketingAI.Application.Features.Buses.Commands.CreateBus;

public class CreateBusRequest
{
    public string PlateNumber { get; set; } = string.Empty;
    public int SeatCapacity { get; set; }
}
