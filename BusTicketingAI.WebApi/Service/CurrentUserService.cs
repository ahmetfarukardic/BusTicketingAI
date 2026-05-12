using BusTicketingAI.Application.Interfaces;
using System.Security.Claims;

namespace BusTicketingAI.WebApi.Service;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return string.IsNullOrEmpty(userIdClaim) ? null : Guid.Parse(userIdClaim);
        }
    }
}
