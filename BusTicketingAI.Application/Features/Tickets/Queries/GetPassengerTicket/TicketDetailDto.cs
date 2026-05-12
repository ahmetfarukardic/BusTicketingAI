namespace BusTicketingAI.Application.Features.Tickets.Queries.GetPassengerTicket;

public record TicketDetailDto(
    Guid TicketId,
    string PassengerName,
    int SeatNumber,
    decimal Price,
    int Status,
    string DepartureTerminal, 
    string ArrivalTerminal,   
    DateTime DepartureTime 
);

public record PassengerTicketResponseDto(
    List<TicketDetailDto> ActiveTickets,
    List<TicketDetailDto> PastTickets
);