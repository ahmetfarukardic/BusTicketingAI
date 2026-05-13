using BusTicketingAI.Domain.Entity;
using MediatR;

namespace BusTicketingAI.Application.Events;

public record TicketPurchasedEvent(
    Guid TicketId,
    string PassengerEmail,
    string PassengerName,
    string PnrCode,
    string OriginTerminal,
    string DestinationTerminal,
    DateTime DepartureTime,
    int SeatNumber,
    decimal Price,
    string CompanyName
) : INotification, IDomainEvent;
