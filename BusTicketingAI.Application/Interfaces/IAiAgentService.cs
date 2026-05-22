namespace BusTicketingAI.Application.Interfaces;

public interface IAiAgentService
{
    Task<string> ProcessUserMessageAsync(string userMessage);
}
