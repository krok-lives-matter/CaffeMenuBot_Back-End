FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /src
COPY CaffeMenuBot.AppHost/CaffeMenuBot.AppHost.csproj CaffeMenuBot.AppHost/
COPY CaffeMenuBot.Data/CaffeMenuBot.Data.csproj CaffeMenuBot.Data/

RUN dotnet restore CaffeMenuBot.AppHost/CaffeMenuBot.AppHost.csproj

COPY . .
WORKDIR /src/CaffeMenuBot.AppHost
RUN dotnet build CaffeMenuBot.AppHost.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish CaffeMenuBot.AppHost.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

EXPOSE 80

ENTRYPOINT ["dotnet", "CaffeMenuBot.AppHost.dll"]