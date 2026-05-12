
using BusTicketingAI.Application.Features.Trips.Commands.CompletePastActiveTrips;
using MediatR;

namespace BusTicketingAI.WebApi.Service;

public class TripStatusUpdaterService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TripStatusUpdaterService> _logger;

    public TripStatusUpdaterService(IServiceProvider serviceProvider, ILogger<TripStatusUpdaterService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Service working...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var updatedCount = await mediator.Send(new CompletePastTripsCommand(), stoppingToken);

                    if (updatedCount > 0)
                    {
                        _logger.LogInformation($"{updatedCount} Past Trip updated with Completed!!!");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service broken");
            }
            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}
