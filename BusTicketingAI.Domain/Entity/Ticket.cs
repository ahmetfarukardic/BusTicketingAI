using System.ComponentModel.DataAnnotations;

namespace BusTicketingAI.Domain.Entity;

public class Ticket : BaseEntity
{
    public Guid Id { get; set; }
    public Guid TripId { get; set; }
    public Guid? UserId { get; set; }
    public string PassengerName { get; set; } = string.Empty;
    public string PassengerTC { get; set; } = string.Empty;
    public int SeatNumber { get; set; }
    public int Status { get; set; }
    public decimal Price { get; set; }
    public string Gender { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public Trip Trip { get; set; } = null!;
    public AppUser? User { get; set; }
}
