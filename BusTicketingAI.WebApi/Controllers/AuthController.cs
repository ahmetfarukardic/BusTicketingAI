using BusTicketingAI.Application.Features.Auth.Commands.ForgotPassword;
using BusTicketingAI.Application.Features.Auth.Commands.Register;
using BusTicketingAI.Application.Features.Auth.Commands.ResetPassword;
using BusTicketingAI.Application.Features.Auth.Queries.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketingAI.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand request)
    {
        var result = await _mediator.Send(request);
        if (result.Token == string.Empty && result.Message.Contains("basarili") == false)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginQuery request)
    {
        var result = await _mediator.Send(request);
        if (string.IsNullOrEmpty(result.Token))
            return Unauthorized(result);

        return Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand request)
    {
        try
        {
            var result = await _mediator.Send(request);
            return Ok(new { Message = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand request)
    {
        try
        {
            await _mediator.Send(request);
            return Ok(new { Message = "Şifreniz başarıyla güncellendi. Yeni şifrenizle giriş yapabilirsiniz." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}