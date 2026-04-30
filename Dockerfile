# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY DiscordBotTest/DiscordBotTest.csproj DiscordBotTest/
RUN dotnet restore DiscordBotTest/DiscordBotTest.csproj

COPY . ./
RUN dotnet publish DiscordBotTest/DiscordBotTest.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "DiscordBotTest.dll"]