using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Domain.Entity;
using BusTicketingAI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BusTicketingAI.Infrastructure.Repository;

public class TripRepository : GenericRepository<Trip>, ITripRepository
{
    public TripRepository(AppDbContext context) : base(context) { }

    public async Task<List<Trip>> SearchTripsAsync(int originCityId, int destinationCityId, DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

        return await _context.Trips
            .Include(t => t.Bus).ThenInclude(b => b.Company)
            .Include(t => t.OriginTerminal)
            .Include(t => t.DestinationTerminal)
            .Where(t => t.OriginTerminal.CityId == originCityId &&
                        t.DestinationTerminal.CityId == destinationCityId &&
                        t.DepartureTime >= startOfDay &&
                        t.DepartureTime <= endOfDay)
            .ToListAsync();
    }

    public async Task<int> GetSoldTicketCountAsync(Guid tripId)
    {
        return await _context.Tickets.Where(t => t.TripId == tripId && t.Status == 1).CountAsync();
    }

    public async Task<List<Trip>> SearchAndPaginateTripsAsync(int? originCityId, int? destinationCityId, DateTime? departureDate, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Trips
            .Include(t => t.Bus).ThenInclude(b => b.Company)
            .Include(t => t.OriginTerminal)
            .Include(t => t.DestinationTerminal)
            .Include(t => t.Tickets)
            .AsQueryable();

        if (originCityId.HasValue)
            query = query.Where(t => t.OriginTerminal.CityId == originCityId.Value);

        if (destinationCityId.HasValue)
            query = query.Where(t => t.DestinationTerminal.CityId == destinationCityId.Value);

        if (departureDate.HasValue)
            query = query.Where(t => t.DepartureTime.Date == departureDate.Value.Date);

        query = query.Where(t => t.DepartureTime >  DateTime.UtcNow);

        return await query.OrderBy(t => t.DepartureTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Trip>> GetTripsByCompanyIdAsync(int companyId, CancellationToken cancellationToken)
    {
        return await _context.Trips
            .Include(t => t.Bus)
            .Include(t => t.OriginTerminal)
            .Include(t => t.DestinationTerminal)
            .Where(t => t.Bus.CompanyId == companyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsBusBelongsToCompanyAsync(int busId, int companyId, CancellationToken cancellationToken)
    {
        return await _context.Buses.AnyAsync(b => b.Id == busId && b.CompanyId == companyId, cancellationToken);
    }

    public async Task<(int TotalBuses, int TodayTrips, int TodayTickets, decimal TodayRevenue)> GetCompanyDailyStatsAsync(int companyId, DateTime date, CancellationToken cancellationToken)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

        var totalBuses = await _context.Buses.CountAsync(b => b.CompanyId == companyId, cancellationToken);
        var todayTrips = await _context.Trips
            .Include(t => t.Tickets)
            .Where(t => t.Bus.CompanyId == companyId &&
                        t.DepartureTime >= startOfDay &&
                        t.DepartureTime <= endOfDay)
            .ToListAsync(cancellationToken);

        var todayTripsCount = todayTrips.Count;
        var todayTicketsCount = todayTrips.SelectMany(t => t.Tickets).Count(t => t.Status == 1);

        var todayRevenue = todayTrips.SelectMany(t => t.Tickets).Where(t => t.Status == 1).Sum(t => t.Trip.BasePrice);

        return (totalBuses, todayTripsCount, todayTicketsCount, todayRevenue);
    }

    public async Task<bool> IsTripBelongsToCompanyAsync(Guid tripId, int companyId, CancellationToken cancellationToken)
    {
        return await _context.Trips.Include(t => t.Bus).AnyAsync(t => t.Id == tripId && t.Bus.CompanyId == companyId, cancellationToken);
    }

    public async Task<List<Trip>> GetActiveTripsByCompanyIdAsync(int companyId, int? originId, int? destinationId, DateTime? date, CancellationToken cancellationToken)
    {
        var query = _context.Trips
            .Include(t => t.Bus)
            .Include(t => t.OriginTerminal)
            .Include(t => t.DestinationTerminal)
            .Where(t => t.Bus.CompanyId == companyId);

        if (date.HasValue)
        {
            query = query.Where(t => t.DepartureTime.Date == date.Value.Date);
        }
        else
        {
            query = query.Where(t => t.DepartureTime >= DateTime.UtcNow);
        }

        if(originId.HasValue)
        {
            query = query.Where(t => t.OriginTerminalId == originId.Value);
        }

        if (destinationId.HasValue)
        {
            query = query.Where(t => t.DestinationTerminalId == destinationId.Value);
        }

        return await query.OrderBy(t => t.DepartureTime).ToListAsync(cancellationToken);
    }

    public async Task<Trip?> GetTripWithBusAsync(Guid tripId, CancellationToken cancellationToken)
    {
        return await _context.Trips.Include(t => t.Bus).FirstOrDefaultAsync(t => t.Id == tripId, cancellationToken);
    }

    public async Task<bool> HasUpcomingTripsByBusIdAsync(int busId, CancellationToken cancellationToken)
    {
        return await _context.Trips.AnyAsync(t => t.BusId == busId && t.DepartureTime >  DateTime.UtcNow, cancellationToken);
    }

    public async Task<Trip?> GetTripWithBusAndTerminalAsync(Guid tripId, CancellationToken cancellationToken)
    {
        return await _context.Trips
            .Include(t => t.DestinationTerminal)
            .Include(t => t.OriginTerminal)
            .Include(t => t.Bus).ThenInclude(b => b.Company)
            .FirstOrDefaultAsync(t => t.Id == tripId, cancellationToken);
    }
}
