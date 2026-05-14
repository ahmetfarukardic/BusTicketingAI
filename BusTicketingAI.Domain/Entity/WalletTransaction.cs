using BusTicketingAI.Domain.Enum;

namespace BusTicketingAI.Domain.Entity;

public class WalletTransaction : BaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public WalletTransactionType TransactionType { get; set; }
    public Guid? ReferenceId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public AppUser User { get; set; } = null!;
}