# 1. Build Aşaması
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Tüm katmanların proje dosyalarını kopyala ve restore et
COPY ["BusTicketingAI.WebApi/BusTicketingAI.WebApi.csproj", "BusTicketingAI.WebApi/"]
COPY ["BusTicketingAI.Application/BusTicketingAI.Application.csproj", "BusTicketingAI.Application/"]
COPY ["BusTicketingAI.Infrastructure/BusTicketingAI.Infrastructure.csproj", "BusTicketingAI.Infrastructure/"]
COPY ["BusTicketingAI.Domain/BusTicketingAI.Domain.csproj", "BusTicketingAI.Domain/"]
RUN dotnet restore "BusTicketingAI.WebApi/BusTicketingAI.WebApi.csproj"

# Kodların tamamını kopyala ve projeyi derle
COPY . .
WORKDIR "/src/BusTicketingAI.WebApi"
RUN dotnet publish "BusTicketingAI.WebApi.csproj" -c Release -o /app/publish

# 2. Çalıştırma Aşaması (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BusTicketingAI.WebApi.dll"]