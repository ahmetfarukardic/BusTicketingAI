using BusTicketingAI.Domain.Entity;

namespace BusTicketingAI.Application.Interfaces;

public interface ITripRepository : IGenericRepository<Trip>
{
    Task<List<Trip>> SearchTripsAsync(int originCityId, int destinationCityId, DateTime date);
    Task<int> GetSoldTicketCountAsync(Guid tripId);
    Task<List<Trip>> SearchAndPaginateTripsAsync(int? originCityId, int? destinationCityId, DateTime? departureDate, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<List<Trip>> GetTripsByCompanyIdAsync(int companyId, CancellationToken cancellationToken);
    Task<bool> IsBusBelongsToCompanyAsync(int busId, int companyId, CancellationToken cancellationToken);
    Task<(int TotalBuses, int TodayTrips, int TodayTickets, decimal TodayRevenue)> GetCompanyDailyStatsAsync(int companyId, DateTime date, CancellationToken cancellationToken);
    Task<bool> IsTripBelongsToCompanyAsync(Guid tripId, int companyId, CancellationToken cancellationToken);
    Task<List<Trip>> GetActiveTripsByCompanyIdAsync(int companyId, int? originId, int? destinationId, DateTime? date, CancellationToken cancellationToken);
    Task<Trip?> GetTripWithBusAsync(Guid tripId, CancellationToken cancellationToken);
    Task<bool> HasUpcomingTripsByBusIdAsync(int busId, CancellationToken cancellationToken);
    Task<Trip?> GetTripWithBusAndTerminalAsync(Guid tripId, CancellationToken cancellationToken);
    Task<List<Trip>> GetPastActiveTripsAsync(DateTime currentTime, CancellationToken cancellationToken);
}
