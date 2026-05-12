using MediatR;

namespace BusTicketingAI.Application.Events;

public record TicketCancelledEvent(
    Guid TicketId,
    string PassengerEmail,
    string PassengerName,
    string PnrCode,
    string OriginTerminal,
    string DestinationTerminal,
    DateTime DepartureTime,
    int SeatNumber,
    string CompanyName
) : INotification;
