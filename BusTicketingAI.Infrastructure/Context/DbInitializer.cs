using System.Text.Json;
using BusTicketingAI.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BusTicketingAI.Infrastructure.Context;

public static class DbInitializer
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider, bool enableSeedData)
    {
        using var scope = serviceProvider.CreateScope();
        var context = serviceProvider.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();

        if (!enableSeedData)
            return;

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

        string[] roles = { "Admin", "CompanyStaff", "Member" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new AppRole { Name = role });
        }

        if (await userManager.FindByEmailAsync("admin@biletprojesi.com") == null)
        {
            var adminUser = new AppUser
            {
                UserName = "admin@biletprojesi.com",
                Email = "admin@biletprojesi.com",
                FirstName = "Sistem",
                LastName = "Yöneticisi",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Admin123*");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        var basePath = Path.Combine(AppContext.BaseDirectory, "Data", "SeedFiles");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // A. Şehirleri Ekle
        if (!await context.Cities.AnyAsync())
        {
            var citiesData = await File.ReadAllTextAsync(Path.Combine(basePath, "Cities.json"));
            var cities = JsonSerializer.Deserialize<List<City>>(citiesData, options);
            if (cities != null)
            {
                await context.Cities.AddRangeAsync(cities);
                await SaveWithIdentityInsertAsync(context, "Cities"); // Özel kayıt metodumuz
            }
        }

        // B. Terminalleri Ekle
        if (!await context.Terminals.AnyAsync())
        {
            var terminalsData = await File.ReadAllTextAsync(Path.Combine(basePath, "Terminals.json"));
            var terminals = JsonSerializer.Deserialize<List<Terminal>>(terminalsData, options);
            if (terminals != null)
            {
                await context.Terminals.AddRangeAsync(terminals);
                await SaveWithIdentityInsertAsync(context, "Terminals");
            }
        }

        // C. Otobüs Firmalarını Ekle
        if (!await context.BusCompanies.AnyAsync())
        {
            var companiesData = await File.ReadAllTextAsync(Path.Combine(basePath, "BusCompanies.json"));
            var companies = JsonSerializer.Deserialize<List<BusCompany>>(companiesData, options);
            if (companies != null)
            {
                await context.BusCompanies.AddRangeAsync(companies);
                await SaveWithIdentityInsertAsync(context, "BusCompanies");
            }
        }

        // D. Otobüsleri Ekle
        if (!await context.Buses.AnyAsync())
        {
            var busesData = await File.ReadAllTextAsync(Path.Combine(basePath, "Buses.json"));
            var buses = JsonSerializer.Deserialize<List<Bus>>(busesData, options);
            if (buses != null)
            {
                await context.Buses.AddRangeAsync(buses);
                await SaveWithIdentityInsertAsync(context, "Buses");
            }
        }
    }

    // --- SİHİRLİ YARDIMCI METOT ---
    // SQL Server'ı geçici olarak susturup bizim belirlediğimiz ID'leri zorla yazdıran metot.
    private static async Task SaveWithIdentityInsertAsync(AppDbContext context, string tableName)
    {
        // Transaction başlatıyoruz ki yarıda kesilirse veritabanı bozulmasın
        using var transaction = await context.Database.BeginTransactionAsync();

        // 1. Otomatik artışı DURDUR
        await context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT {tableName} ON");

        // 2. Bizim JSON verilerini kaydet
        await context.SaveChangesAsync();

        // 3. Otomatik artışı tekrar BAŞLAT (Veritabanı normale dönsün)
        await context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT {tableName} OFF");

        // İşlemi onayla
        await transaction.CommitAsync();
    }
}