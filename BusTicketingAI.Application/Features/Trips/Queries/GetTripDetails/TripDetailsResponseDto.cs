using BusTicketingAI.Application.Features.Tickets.Queries.GetOccupiedSeats;

namespace BusTicketingAI.Application.Features.Trips.Queries.GetTripDetails;

public record AvailableSeatDto(int SeatNumber, string Description);

public record TripDetailsResponseDto(decimal Price, int SeatCapacity, List<OccupiedSeatDto> OccupiedSeats, List<AvailableSeatDto> AvailableSeats);