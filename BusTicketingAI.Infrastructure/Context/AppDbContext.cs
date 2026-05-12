using BusTicketingAI.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BusTicketingAI.Infrastructure.Context;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Terminal> Terminals { get; set; }
    public DbSet<BusCompany> BusCompanies { get; set; }
    public DbSet<Bus> Buses { get; set; }
    public DbSet<Trip> Trips { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>().ToTable("Users");
        builder.Entity<AppRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        builder.Entity<Trip>()
            .HasOne(t => t.OriginTerminal)
            .WithMany()
            .HasForeignKey(t => t.OriginTerminalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Trip>()
            .HasOne(t => t.DestinationTerminal)
            .WithMany()
            .HasForeignKey(t => t.DestinationTerminalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Ticket>()
            .Property(t => t.Price)
            .HasColumnType("decimal(18,2)");
        builder.Entity<Trip>()
            .Property(t => t.BasePrice)
            .HasColumnType("decimal(18,2)");
    }
}
