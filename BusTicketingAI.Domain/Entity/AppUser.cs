using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BusTicketingAI.Domain.Entity;

public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;

    [StringLength(11, MinimumLength = 11)]
    public string? IdentityNumber { get; set; }
    public DateTime RegisteredAt { get; set; }
    public int? CompanyId { get; set; }
}

public class AppRole : IdentityRole<Guid>
{
}
