using BusTicketingAI.Application.Features.Tickets.Queries.GetOccupiedSeats;

namespace BusTicketingAI.Application.Features.Trips.Queries.GetTripDetails;

public record TripDetailsResponseDto(decimal Price, int SeatCapacity, List<OccupiedSeatDto> OccupiedSeats);