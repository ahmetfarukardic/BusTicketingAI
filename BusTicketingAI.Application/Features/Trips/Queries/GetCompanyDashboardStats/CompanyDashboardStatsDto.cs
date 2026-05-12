namespace BusTicketingAI.Application.Features.Trips.Queries.GetCompanyDashboardStats;

public record CompanyDashboardStatsDto(
    int TotalBuses,
    int TodayTotalTrips,
    int TodaySoldTickets,
    decimal TodayRevenue
);