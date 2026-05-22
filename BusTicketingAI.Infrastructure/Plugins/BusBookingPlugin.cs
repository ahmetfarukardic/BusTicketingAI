using BusTicketingAI.Application.Features.Tickets.Commands.LockSeats;
using BusTicketingAI.Application.Features.Tickets.Queries.GetOccupiedSeats;
using BusTicketingAI.Application.Features.Trips.Queries.GetTripDetails;
using BusTicketingAI.Application.Features.Trips.Queries.SearchTrips;
using BusTicketingAI.Application.Features.Users.Queries.GetUserProfile;
using BusTicketingAI.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Security.Claims;
using System.Text.Json;
using System.Globalization;

namespace BusTicketingAI.Infrastructure.Plugins;

public class BusBookingPlugin 
{
    private readonly IMediator _mediator;
    private readonly ICityRepository _cityRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BusBookingPlugin(IMediator mediator, ICityRepository cityRepository, IHttpContextAccessor httpContextAccessor)
    {
        _mediator = mediator;
        _cityRepository = cityRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    [KernelFunction("SearchTrips")]
    [Description("Kullanıcının verdiği kalkış ve varış şehirlerine (isim olarak) ve tarihe göre aktif otobüs seferlerini bulur. Seferlerin saatlerini, fiyatlarını ve TripId (Sefer ID) değerlerini döndürür.")]
    public async Task<string> SearchTripsAsync(
        [Description("Kalkış şehri adı. Örn: Ankara")] string originCityName,
        [Description("Varış şehri adı. Örn: Bursa")] string destinationCityName,
        [Description("Tarih.KESINLIKLE YYYY-MM-DD formatında. Eğer kullanıcı spesifik bir tarih belirtmezse ('en yakın', 'ne zaman var' gibi) bu alanı KESİNLİKLE BOŞ BIRAK.")] string date = "")
    {
        try
        {
            var allCities = await _cityRepository.GetAllAsync(cancellationToken: default);
            var originCity = allCities.FirstOrDefault(c => NormalizeTR(c.Name) == NormalizeTR(originCityName));
            var destinationCity = allCities.FirstOrDefault(c => NormalizeTR(c.Name) == NormalizeTR(destinationCityName));

            if (originCity == null || destinationCity == null)
                return $"Belirtilenşehirler sistemde bulunamadı. Lütfen geçerli şehir isimleri girin. Aranan: {originCityName} - {destinationCityName}";

            DateTime? queryDate = null;
            if (string.IsNullOrWhiteSpace(date))
            {
                if (DateTime.TryParse(date, new CultureInfo("tr-TR"), DateTimeStyles.None, out DateTime parsedDate) ||
                    DateTime.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                {
                    queryDate = parsedDate;
                }
            }

            var query = new SearchTripsQuery
            {
                OriginCityId = originCity.Id,
                DestinationCityId = destinationCity.Id,
                DepartureDate = queryDate,
                PageNumber = 1,
                PageSize = 10,
            };

            var result = await _mediator.Send(query);

            if (result == null || !result.Any())
                return "Belirtilen kriterlere uygun sefer bulunamadı.";

            return JsonSerializer.Serialize(result);
        }
        catch (Exception ex)
        {
            return $"Sefer aranırken hata oluştu: {ex.Message}";
        }
    }

    [KernelFunction("GetTripDetails")]
    [Description("Belirli bir seferin (TripId) detaylarını, kapasitesini ve dolu/rezerve olan koltuk numaralarını getirir. Kullanıcı koltuk seçmek istediğinde boş koltukları bulmak için mutlaka kullanılmalıdır.")]
    public async Task<string> GetTripDetailsAsync([Description("Detayları getirilecek seferin benzersiz ID'si (Guid).")] string tripId)
    {
        try
        {
            if (!Guid.TryParse(tripId, out Guid parsedTripId))
                return "Geçersiz Sefer ID formatı.";

            var query = new GetTripDetailsQuery(parsedTripId);
            var result = await _mediator.Send(query);

            return JsonSerializer.Serialize(result);
        }
        catch (Exception ex)
        {
            return $"Sefer detayları alınırken hata oluştu: {ex.Message}";
        }
    }

    [KernelFunction("CheckUserProfileStatus")]
    [Description("Mevcut giriş yapmış kullanıcının profil bilgilerinin (TC Kimlik, Cinsiyet, Telefon vb.) eksiksiz olup olmadığını kontrol eder. Eğer bilgiler tamsa 'Complete', eksikse 'Incomplete' döner.")]
    public async Task<string> CheckUserProfileStatusAsync()
    {
        try
        {
            var userClaimId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userClaimId))
                return "UserNotLoggedIn";

            var userId = Guid.Parse(userClaimId);
            var profile = await _mediator.Send(new GetUserProfileQuery(userId));

            bool isComplete = !string.IsNullOrWhiteSpace(profile.PhoneNumber) && 
                              !string.IsNullOrWhiteSpace(profile.Gender) && 
                              !string.IsNullOrWhiteSpace(profile.IdentityNumber);

            return isComplete ? "Complete" : "Incomplete";
        }
        catch (Exception ex)
        {
            return $"Profil kontrolünde hata: {ex.Message}";
        }
    }

    [KernelFunction("LockSingleSeatForCheckout")]
    [Description("Kullanıcı TEK BİR KOLTUK seçtiğinde ve profil bilgileri TAM olduğunda, o koltuğu ödeme adımı için kilitler. İşlem başarılı olursa 'Success' döner.")]
    public async Task<string> LockSingleSeatForCheckoutAsync(
        [Description("Seçilen seferin benzersiz ID'si (Guid)")] string tripId,
        [Description("Seçilen koltuk numarası")] int seatNumber)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return "UserNotLoggedIn";

            if (!Guid.TryParse(tripId, out Guid parsedTripId)) return "InvalidTripId";

            var userId = Guid.Parse(userIdClaim);

            var profile = await _mediator.Send(new GetUserProfileQuery(userId));

            var seatDto = new OccupiedSeatDto(seatNumber, profile.Gender);

            var command = new LockSeatsCommand(parsedTripId, new List<OccupiedSeatDto> { seatDto });

            var (IsSuccess, Message) = await _mediator.Send(command);

            return IsSuccess ? "Success" : $"Failed : {Message}";
        }
        catch (Exception ex)
        {
            return $"Kilit işlemi başarısız: {ex.Message}";
        }
    }

    private string NormalizeTR(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        return text.ToLower(new CultureInfo("tr-TR"))
                   .Replace("ş", "s").Replace("ı", "i").Replace("ğ", "g")
                   .Replace("ç", "c").Replace("ö", "o").Replace("ü", "u");
    }
}