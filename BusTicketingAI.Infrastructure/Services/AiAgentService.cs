using BusTicketingAI.Application.Interfaces;
using BusTicketingAI.Infrastructure.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace BusTicketingAI.Infrastructure.Services;

public class AiAgentService : IAiAgentService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;

    private static readonly ChatHistory _chatHistory = new ChatHistory();
    private static bool _isSystemPromptAdded = false;

    public AiAgentService(Kernel kernel, BusBookingPlugin busBookingPlugin)
    {
        _kernel = kernel;

        _kernel.Plugins.AddFromObject(busBookingPlugin);
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

        if (!_isSystemPromptAdded)
        {
            string todaysDate = DateTime.Now.ToString("dd MMMM yyyy dddd");
            _chatHistory.AddSystemMessage(
                $"Sen BusTicketingAI bilet satış asistanısın. ŞU ANKİ TARİH: {todaysDate}. " +
                "Kullanıcıya sefer bul ve koltuk seçmesinde yardımcı ol. " +
                "0. Sefer araması yapabilmen için kullanıcının HEM KALKIŞ HEM DE VARIŞ şehrini belirtmesi zorunludur. Eğer şehirlerden biri eksikse, KESİNLİKLE 'SearchTrips' metodunu çağırma ve kullanıcıya eksik olan şehri (Nereden veya Nereye gideceğini) sor.\n" +
                "KULLANICI BİR VEYA BİRDEN FAZLA KOLTUK SEÇTİĞİNDE ŞU KURALLARI UYGULA:\n" +
                "1. Eğer kullanıcı BİRDEN FAZLA koltuk seçtiyse, doğrudan şu JSON formatında cevap ver: {\"action\": \"NAVIGATE_TO_SEATS\", \"tripId\": \"sefer-id\"}\n" +
                "2. Eğer kullanıcı SADECE TEK BİR koltuk seçtiyse, önce 'CheckUserProfileStatus' metodunu kullanarak profilinin tam olup olmadığını kontrol et.\n" +
                "3. Profil eksikse ('Incomplete' dönerse), kullanıcıya kibarca profil bilgilerinin eksik olduğunu belirt ve şu JSON ile koltuk ekranına yönlendir: {\"action\": \"NAVIGATE_TO_SEATS\", \"tripId\": \"sefer-id\"}\n" +
                "4. Profil tamsa ('Complete' dönerse), SADECE 'LockSingleSeatForCheckout' metodunu çalıştır. Eğer bu metot 'Success' dönerse, kullanıcıyı SADECE şu JSON ile doğrudan ÖDEME sayfasına yönlendir: " +
                "{\"action\": \"NAVIGATE_TO_PAYMENT\", \"tripId\": \"sefer-id\", \"seatNumber\": secilen-koltuk-no, \"gender\": \"kullanici-cinsiyeti\", \"price\": sefer-fiyati}\n" +
                "DİKKAT: Yönlendirme yaptığın durumlarda SADECE JSON komutunu yaz, metin veya yorum ekleme.");

            _isSystemPromptAdded = true;
        }
    }

    public async Task<string> ProcessUserMessageAsync(string userMessage)
    {
        _chatHistory.AddUserMessage(userMessage);

        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        var response = await _chatCompletionService.GetChatMessageContentsAsync(_chatHistory, executionSettings, _kernel);
        var reply = response.FirstOrDefault()?.Content ?? "Cevap alınamadı.";

        _chatHistory.AddAssistantMessage(reply);

        return reply;
    }
}
