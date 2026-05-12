namespace BusTicketingAI.Application.Features.Users;

public record CreateUserDto(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Role,
    int? CompanyId
);