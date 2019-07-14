FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["API/ExchangeRatesGateway.API.csproj", "API/"]
COPY ["Domain/ExchangeRatesGateway.Domain.csproj", "Domain/"]
RUN dotnet restore "API/ExchangeRatesGateway.API.csproj"
COPY . .
WORKDIR "/src/API"
RUN dotnet build "ExchangeRatesGateway.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ExchangeRatesGateway.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ExchangeRatesGateway.API.dll"]