using BusTicketingAI.Application.Features.BusCompanies;
using BusTicketingAI.Application.Features.BusCompanies.Commands;
using BusTicketingAI.Application.Features.BusCompanies.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketingAI.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class BusCompaniesController : ControllerBase
{
    private readonly IMediator _mediator;
    public BusCompaniesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllBusCompaniesQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var result = await _mediator.Send(new GetBusCompanyByIdQuery(id));
        if (result == null)
            return NotFound(new { message = "Firma bulunamadı!" });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBusCompanyDto dto)
    {
        var newId = await _mediator.Send(new CreateBusCompanyCommand(dto.Name));
        return Ok(new { id = newId, message = "Firma başarıyla eklendi." });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateBusCompanyDto dto)
    {
        if (id != dto.Id)
            return BadRequest(new {message = "URL ve paket içindeki ID uyuşmuyor!" });

        await _mediator.Send(new UpdateBusCompanyCommand(dto.Id, dto.Name));
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _mediator.Send(new DeleteBusCompanyCommand(id));
        return NoContent();
    }
}
