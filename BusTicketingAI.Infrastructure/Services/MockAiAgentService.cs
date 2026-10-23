using BusTicketingAI.Application.Interfaces;

namespace BusTicketingAI.Infrastructure.Services;

public class MockAiAgentService : IAiAgentService
{
    public Task<string> ProcessUserMessageAsync(string userMessage)
    {
        return Task.FromResult("AI asistanı test ve CI/CD süreçleri için geçici olarak devre dışı bırakılmıştır. Lütfen işleminizi manuel olarak bilet arama ekranından gerçekleştirin.");
    }
}
