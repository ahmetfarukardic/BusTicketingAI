using BusTicketingAI.Application.DTOs;
using BusTicketingAI.Application.Events;
using BusTicketingAI.Application.Features.Tickets.Queries.GetOccupiedSeats;
using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Domain.Entity;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace BusTicketingAI.Application.Features.Tickets.Commands.CheckoutTicket;

public record CheckoutCommand(
    Guid TripId, 
    Guid? UserId,
    string ContactEmail,
    string ContactPhone,
    List<PassengerDetailDto> Passengers, 
    string CardHolderName, 
    string CardNumber, 
    string ExpirationDate, 
    string Cvv, 
    decimal TotalAmount
) : IRequest<Guid>;

public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, Guid>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IPaymentService _paymentService;
    private readonly IMemoryCache _memoryCache;

    public CheckoutCommandHandler(ITicketRepository ticketRepository, IPaymentService paymentService, ITripRepository tripRepository, IMemoryCache memoryCache)
    {
        _ticketRepository = ticketRepository;
        _tripRepository = tripRepository;
        _paymentService = paymentService;
        _memoryCache = memoryCache;
    }

    public async Task<Guid> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        foreach (var passenger in request.Passengers)
        {
            var isTaken = await _ticketRepository.IsSeatTakenAsync(request.TripId, passenger.SeatNumber, cancellationToken);
            if (isTaken)
                throw new Exception($"Üzgünüz, {passenger.SeatNumber} numaralı koltuk kısa süre önce satılmıştır.");
        }

        var paymentRequest = new PaymentRequest(request.CardHolderName, request.CardNumber, request.ExpirationDate, request.Cvv, request.TotalAmount);
        var paymentResult = await _paymentService.ProcessPaymentAsync(paymentRequest, cancellationToken);

        if (!paymentResult.IsSuccessful)
            throw new Exception(paymentResult.ErrorMessage);

        var trip = await _tripRepository.GetTripWithBusAndTerminalAsync(request.TripId, cancellationToken) ?? throw new Exception("Sefer bulunamadı.");

        foreach (var passenger in request.Passengers)
        {
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                TripId = request.TripId,
                UserId = request.UserId,
                PassengerName = passenger.FullName,
                PassengerTC = passenger.TCIdentity,
                SeatNumber = passenger.SeatNumber,
                Gender = passenger.Gender,
                Price = trip.BasePrice,
                Status = 1,
                CreatedAt = DateTime.UtcNow
            };
            string pnrCode = ticket.Id.ToString()[..8].ToUpper();

            ticket.AddDomainEvent(new TicketPurchasedEvent(
                TicketId: ticket.Id,
                PassengerEmail: request.ContactEmail,
                PassengerName: ticket.PassengerName,
                PnrCode: pnrCode,
                OriginTerminal: trip.OriginTerminal.Name,
                DestinationTerminal: trip.DestinationTerminal.Name,
                DepartureTime: trip.DepartureTime,
                SeatNumber: ticket.SeatNumber,
                Price: ticket.Price,
                CompanyName: trip.Bus.Company.Name
            ));
            await _ticketRepository.AddAsync(ticket, cancellationToken);
        }

        await _ticketRepository.SaveChangesAsync(cancellationToken);
        var cacheKey = $"Trip_{request.TripId}_Locks";
        if (_memoryCache.TryGetValue(cacheKey, out List<OccupiedSeatDto>? lockedSeats) && lockedSeats != null)
        {
            lockedSeats.RemoveAll(ls => request.Passengers.Any(p => p.SeatNumber == ls.SeatNumber));
            _memoryCache.Set(cacheKey, lockedSeats, TimeSpan.FromMinutes(5));
        }

        return request.TripId;
    }
}
