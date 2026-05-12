namespace BusTicketingAI.Application.Features.Tickets.Commands.SellTicket;

public class SellTicketRequestDto
{
    public Guid TripId { get; set; }
    public int SeatNumber { get; set; }
    public string PassengerName { get; set; } = string.Empty;
    public string PassengerTC { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
