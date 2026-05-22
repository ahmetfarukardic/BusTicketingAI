using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.Users.Commands.UpdateMyProfile;

public record UpdateUserProfileCommand(Guid UserId, string IdentityNumber, string PhoneNumber, string Gender) : IRequest<(bool IsSuccess, string Message)>;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, (bool IsSuccess, string Message)>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserProfileCommandHandler(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task<(bool IsSuccess, string Message)> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
            return (false, "Kullanıcı bulunamadı.");

        if (!string.IsNullOrEmpty(request.IdentityNumber) && request.IdentityNumber.Length != 11)
            return (false, "T.C. Kimlik numarası 11 haneli olmalıdır.");

        user.IdentityNumber = request.IdentityNumber;
        user.PhoneNumber = request.PhoneNumber;
        user.Gender = request.Gender;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return (true, "Profil bilgileriniz başarıyla güncellendi.");
    }
}
