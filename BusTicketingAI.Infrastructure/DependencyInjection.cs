using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Infrastructure.Plugins;
using BusTicketingAI.Infrastructure.Repository;
using BusTicketingAI.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace BusTicketingAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
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
        services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();

        //var modelId = configuration["OpenAI:ModelId"];
        //var apiKey = configuration["OpenAI:ApiKey"];

        //services.AddKernel().AddOpenAIChatCompletion(modelId, apiKey);
        //services.AddScoped<BusBookingPlugin>();
        //services.AddScoped<IAiAgentService, AiAgentService>();
        services.AddScoped<IAiAgentService, MockAiAgentService>();

        return services;
    }
}
