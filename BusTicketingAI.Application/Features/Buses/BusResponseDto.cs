namespace BusTicketingAI.Application.Features.Buses;

public record BusResponseDto(
    int Id,
    string PlateNumber,
    int SeatCapacity,
    int CompanyId
);