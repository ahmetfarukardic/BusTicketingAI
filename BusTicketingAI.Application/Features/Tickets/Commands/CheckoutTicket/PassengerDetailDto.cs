namespace BusTicketingAI.Application.Features.Tickets.Commands.CheckoutTicket;

public record PassengerDetailDto(int SeatNumber, string FullName, string TCIdentity, string Gender);