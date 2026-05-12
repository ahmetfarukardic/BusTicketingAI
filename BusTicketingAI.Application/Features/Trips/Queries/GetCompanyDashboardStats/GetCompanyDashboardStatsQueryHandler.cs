using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.Trips.Queries.GetCompanyDashboardStats;

public record GetCompanyDashboardStatsQuery(int CompanyId) : IRequest<CompanyDashboardStatsDto>;

public class GetCompanyDashboardStatsQueryHandler : IRequestHandler<GetCompanyDashboardStatsQuery, CompanyDashboardStatsDto>
{
    private readonly ITripRepository _tripRepository;

    public GetCompanyDashboardStatsQueryHandler(ITripRepository tripRepository) => _tripRepository = tripRepository;
    
    public async Task<CompanyDashboardStatsDto> Handle(GetCompanyDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var stats = await _tripRepository.GetCompanyDailyStatsAsync(request.CompanyId, DateTime.UtcNow, cancellationToken);

        return new CompanyDashboardStatsDto(
            stats.TotalBuses,
            stats.TodayTrips,
            stats.TodayTickets,
            stats.TodayRevenue
        );
    }
}
