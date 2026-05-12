using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Domain.Entity;
using BusTicketingAI.Infrastructure.Context;

namespace BusTicketingAI.Infrastructure.Repository;

public class UserRepository : GenericRepository<AppUser>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }
}
