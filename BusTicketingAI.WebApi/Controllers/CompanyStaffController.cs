using BusTicketingAI.Application.Features.Buses.Commands.CreateBus;
using BusTicketingAI.Application.Features.Buses.Commands.DeleteBus;
using BusTicketingAI.Application.Features.Buses.Queries.GetCompanyBuses;
using BusTicketingAI.Application.Features.Tickets.Commands.CancelTicket;
using BusTicketingAI.Application.Features.Tickets.Queries.GetOccupiedSeats;
using BusTicketingAI.Application.Features.Tickets.Queries.GetTripPassengers;
using BusTicketingAI.Application.Features.Trips;
using BusTicketingAI.Application.Features.Trips.Commands.CreateTrip;
using BusTicketingAI.Application.Features.Trips.Commands.UpdateTripTime;
using BusTicketingAI.Application.Features.Trips.Queries.GetCompanyActiveTrips;
using BusTicketingAI.Application.Features.Trips.Queries.GetCompanyDashboardStats;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketingAI.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "CompanyStaff")]
public class CompanyStaffController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompanyStaffController(IMediator mediator) => _mediator = mediator;

    [HttpGet("daily-stats")]
    public async Task<IActionResult> GetDailyStats([FromQuery] DateTime date, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirst("CompanyId");
        if (companyIdClaim == null || !int.TryParse(companyIdClaim.Value, out int companyId))
        {
            return Forbid();
        }
        var query = new GetCompanyDashboardStatsQuery(companyId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("active-trips")]
    public async Task<IActionResult> GetActiveTrips([FromQuery] int? originId, [FromQuery] int? destinationId, [FromQuery] DateTime? date, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirst("CompanyId");
        if (companyIdClaim == null || !int.TryParse(companyIdClaim.Value, out int companyId))
        {
            return Forbid();
        }

        var query = new GetCompanyActiveTripsQuery(companyId, originId, destinationId, date);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("trips/{tripId}/occupied-seats")]
    public async Task<IActionResult> GetOccupiedSeats([FromRoute] Guid TripId, CancellationToken cancellationToken)
    {
        var query = new GetOccupiedSeatsQuery(TripId);
        var result = await _mediator.Send(query,cancellationToken);
        return Ok(result);
    }

    [HttpGet("buses")]
    public async Task<IActionResult> GetCompanyBuses(CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirst("CompanyId");
        if (companyIdClaim == null || !int.TryParse(companyIdClaim.Value, out int companyId))
        {
            return Forbid();
        }

        var query = new GetBusesByCompanyQuery(companyId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpPost("trips")]
    public async Task<IActionResult> CreateTrip([FromBody] CreateTripRequest request, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirst("CompanyId");
        if (companyIdClaim == null || !int.TryParse(companyIdClaim.Value, out int companyId))
        {
            return Forbid();
        }

        var command = new CreateTripCommand(
            request.BusId,
            request.OriginTerminalId,
            request.DestinationTerminalId,
            request.DepartureTime,
            request.BasePrice,
            request.EstimatedDuration,
            companyId
        );

        try
        {
            var tripId = await _mediator.Send(command, cancellationToken);
            return Ok(new { TripId = tripId, Message = "Sefer başarıyla oluşturuldu." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("trips/{id}/time")]
    public async Task<IActionResult> UpdateTripTime(Guid id, [FromBody] UpdateTripTimeRequest request, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirst("CompanyId");
        if (companyIdClaim == null || !int.TryParse(companyIdClaim.Value, out int companyId))
        {
            return Forbid();
        }

        var command = new UpdateTripTimeCommand(id, request.NewDepartureTime, companyId, cancellationToken);

        try
        {
            await _mediator.Send(command, cancellationToken);
            return Ok(new { Message = "Sefer saati başarıyla güncellendi." });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch(InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("buses")]
    public async Task<IActionResult> CreateBus([FromBody] CreateBusRequest request, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirst("CompanyId");
        if (companyIdClaim == null || !int.TryParse(companyIdClaim.Value, out int companyId))
        {
            return Forbid();
        }
        var command = new CreateBusCommand(request.PlateNumber, request.SeatCapacity, companyId);
        try
        {
            var busId = await _mediator.Send(command, cancellationToken);
            return Ok(new { BusId = busId, Message = "Otobüs filoya başarıyla eklendi." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("buses/{id}")]
    public async Task<IActionResult> DeleteBus(int id, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirst("CompanyId");
        if (companyIdClaim == null || !int.TryParse(companyIdClaim.Value, out int companyId))
        {
            return Forbid();
        }
        var command = new DeleteBusCommand(id, companyId);
        try
        {
            await _mediator.Send(command, cancellationToken);
            return Ok(new { Message = "Otobüs filodan başarıyla silindi." });
        }
        catch (UnauthorizedAccessException ex) 
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("tickets/{ticketId}/cancel")]
    public async Task<IActionResult> CancelTicket(Guid ticketId, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirst("CompanyId");
        if (companyIdClaim == null || !int.TryParse(companyIdClaim.Value, out int companyId))
            return Forbid();

        var command = new CancelTicketCommand(ticketId, companyId);
        try
        {
            await _mediator.Send(command, cancellationToken);
            return Ok(new { Message = "Bilet başarıyla iptal edildi ve koltuk boşa çıkarıldı." });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("trips/{tripId}/passengers")]
    public async Task<IActionResult> GetTripPassengers(Guid tripId, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirst("CompanyId");
        if (companyIdClaim == null || !int.TryParse(companyIdClaim.Value, out int companyId))
            return Forbid();

        var result = await _mediator.Send(new GetTripPassengersQuery(tripId, companyId), cancellationToken);
        return Ok(result);
    }
}