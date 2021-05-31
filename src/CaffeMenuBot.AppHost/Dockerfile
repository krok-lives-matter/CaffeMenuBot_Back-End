FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
RUN apt-get -y update && apt-get -y install apt-utils && apt-get -y install libgdiplus
FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build


WORKDIR /src
COPY CaffeMenuBot.AppHost/CaffeMenuBot.AppHost.csproj CaffeMenuBot.AppHost/
COPY CaffeMenuBot.Data/CaffeMenuBot.Data.csproj CaffeMenuBot.Data/
COPY CaffeMenuBot.Bot/CaffeMenuBot.Bot.csproj CaffeMenuBot.Bot/

RUN dotnet restore CaffeMenuBot.AppHost/CaffeMenuBot.AppHost.csproj

COPY . .
WORKDIR /src/CaffeMenuBot.AppHost
RUN dotnet build CaffeMenuBot.AppHost.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish CaffeMenuBot.AppHost.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "CaffeMenuBot.AppHost.dll"]
