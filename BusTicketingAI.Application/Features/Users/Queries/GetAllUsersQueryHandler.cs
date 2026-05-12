using BusTicketingAI.Application.DTOs;
using BusTicketingAI.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusTicketingAI.Application.Features.Users.Queries;

public record GetAllUsersQuery : IRequest<List<UserResponseDto>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserResponseDto>>
{
    private readonly UserManager<AppUser> _userManager;

    public GetAllUsersQueryHandler(UserManager<AppUser> userManager) => _userManager = userManager;

    public async Task<List<UserResponseDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userManager.Users.ToListAsync(cancellationToken);
        var responseList = new List<UserResponseDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var currentRole = roles.FirstOrDefault() ?? "Member";

            responseList.Add(new UserResponseDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email ?? "",
                currentRole,
                user.CompanyId
            ));
        }
        return responseList;
    }
}
