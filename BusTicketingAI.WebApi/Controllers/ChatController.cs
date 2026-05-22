using BusTicketingAI.Application.DTOs;
using BusTicketingAI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketingAI.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly IAiAgentService _aiAgentService;

    public ChatController(IAiAgentService aiAgentService) => _aiAgentService = aiAgentService;

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] ChatRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new { Message = "Mesaj alanı boş olamaz." });

        try
        {
            var reply = await _aiAgentService.ProcessUserMessageAsync(request.Message);
            return Ok(new { Reply = reply });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new {Message = $"Yapay zeka ile iletişim kurulurken bir hata oluştu: {ex.Message}" });
        }
    }
}
