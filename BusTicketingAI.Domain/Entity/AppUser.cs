using Microsoft.AspNetCore.Identity;

namespace BusTicketingAI.Domain.Entity;

public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
    public int? CompanyId { get; set; }
}

public class AppRole : IdentityRole<Guid>
{
}
