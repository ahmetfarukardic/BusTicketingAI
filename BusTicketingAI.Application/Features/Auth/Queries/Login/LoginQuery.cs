using MediatR;

namespace BusTicketingAI.Application.Features.Auth.Queries.Login;

public class LoginQuery : IRequest<AuthResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
