FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build

WORKDIR /src
COPY CaffeMenuBot.Client/CaffeMenuBot.Client.csproj CaffeMenuBot.Client/
COPY CaffeMenuBot.Shared/CaffeMenuBot.Shared.csproj CaffeMenuBot.Shared/

RUN dotnet restore CaffeMenuBot.Client/CaffeMenuBot.Client.csproj

COPY . .
WORKDIR /src/CaffeMenuBot.Client
RUN dotnet build CaffeMenuBot.Client.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish CaffeMenuBot.Client.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

EXPOSE 80

ENTRYPOINT ["dotnet", "CaffeMenuBot.Client.dll"]
