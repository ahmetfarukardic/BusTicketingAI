using BusTicketingAI.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BusTicketingAI.Infrastructure.Context;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    private readonly IMediator _mediator;
    public AppDbContext(DbContextOptions options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Terminal> Terminals { get; set; }
    public DbSet<BusCompany> BusCompanies { get; set; }
    public DbSet<Bus> Buses { get; set; }
    public DbSet<Trip> Trips { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entitiesWithEvents.ForEach(e => e.ClearDomainEvent());
        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish((INotification)domainEvent, cancellationToken);
        }
        return result;
    }

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
