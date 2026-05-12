using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Infrastructure.Repository;
using BusTicketingAI.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BusTicketingAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<ITripRepository, TripRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IBusCompanyRepository, BusCompanyRepository>();
        services.AddScoped<IBusRepository, BusRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<ITerminalRepository, TerminalRepository>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }
}
