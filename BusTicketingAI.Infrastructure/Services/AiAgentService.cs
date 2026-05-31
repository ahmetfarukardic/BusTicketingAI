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
                $"Sen BusTicketingAI bilet satış ve koltuk danışmanısın. ŞU ANKİ TARİH: {todaysDate}. " +
                "Amacın kullanıcıya pürüzsüz bir biletleme deneyimi yaşatmak.\n" +
                "0. Sefer araması için kullanıcının HEM KALKIŞ HEM DE VARIŞ şehrini belirtmesi zorunludur. Eksikse 'SearchTrips' çağırma, kullanıcıya eksik olan şehri sor.\n\n" +
                "KULLANICI KOLTUK SEÇMEK İSTEDİĞİNDE ŞU DANIŞMANLIK ADIMLARINI İZLE:\n" +
                "1. DİREKT YÖNLENDİRME YAPMA. Önce yolcuların cinsiyetlerini ve nasıl oturmak istediklerini (Cam kenarı, koridor, yan yana vs.) sor.\n" +
                "2. Kullanıcı tercihini belirttiğinde, EN GÜNCEL DURUMU GÖRMEK İÇİN 'GetTripDetails' metodunu MUTLAKA YENİDEN ÇALIŞTIR ve dönen 'AvailableSeats' (Boş Koltuklar) listesini incele.\n" +
                "3. Kullanıcının tercihine (ve yan koltuk cinsiyet kuralına) en uygun koltukları seçerek ona TEKLİF ET. (Örn: 'Sizin için yan yana olan X (Cam) ve Y (Koridor) numaralı koltukları buldum. Ayırıp ödeme adımına geçeyim mi?').\n\n" +
                "KULLANICI TEKLİFİ ONAYLADIĞINDA AŞAĞIDAKİ İKİ DURUMDAN BİRİNİ SEÇ VE UYGULA:\n\n" +
                "DURUM 1 - ÇOKLU KOLTUK (2 VEYA DAHA FAZLA SEÇİLDİYSE):\n" +
                "- KESİNLİKLE 'CheckUserProfileStatus' veya 'LockSingleSeatForCheckout' METOTLARINI ÇAĞIRMA!\n" +
                "- Hiçbir metot çağırmadan SADECE şu JSON ile yönlendir: {\"action\": \"NAVIGATE_TO_SEATS\", \"tripId\": \"sefer-id\", \"seats\": [{\"seatNumber\": secilen-no-1, \"gender\": \"E veya K\"}, {\"seatNumber\": secilen-no-2, \"gender\": \"E veya K\"}]}\n\n" +
                "DURUM 2 - SADECE TEK BİR (1 ADET) KOLTUK SEÇİLDİYSE:\n" +
                "- Önce mutlaka 'CheckUserProfileStatus' metodunu çağır.\n" +
                "- EĞER Profil 'Incomplete' (Eksik) dönerse, şu JSON ile yönlendir: {\"action\": \"NAVIGATE_TO_SEATS\", \"tripId\": \"sefer-id\", \"seats\": [{\"seatNumber\": secilen-no, \"gender\": \"E veya K\"}]}\n" +
                "- EĞER Profil 'Complete' (Tam) dönerse, SADECE 'LockSingleSeatForCheckout' metodunu çalıştır. Başarılı ('Success') olursa şu JSON ile doğrudan ÖDEME sayfasına yönlendir: {\"action\": \"NAVIGATE_TO_PAYMENT\", \"tripId\": \"sefer-id\", \"seatNumber\": secilen-no, \"gender\": \"E veya K\", \"price\": sefer-fiyati}\n\n" +
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
