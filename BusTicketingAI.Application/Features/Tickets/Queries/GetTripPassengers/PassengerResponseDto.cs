namespace BusTicketingAI.Application.Features.Tickets.Queries.GetTripPassengers;

public record PassengerResponseDto(Guid TicketId, string PassengerName, string IDentityNumber, int SeatNumber, int Status);