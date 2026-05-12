namespace BusTicketingAI.Application.Features.Users.Queries.GetUserProfile;

public record UserProfileDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber
);