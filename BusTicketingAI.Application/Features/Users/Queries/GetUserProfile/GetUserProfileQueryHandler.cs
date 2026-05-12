using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.Users.Queries.GetUserProfile;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserProfileDto>;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserProfileQueryHandler(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        return user == null
            ? throw new Exception("Kullanıcı bulunamadı.")
            : new UserProfileDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email ?? string.Empty,
            user.PhoneNumber ?? string.Empty
            );
    }
}
