FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish ChatbotConversacional.Storage.Api/ChatbotConversacional.Storage.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
USER root
RUN mkdir -p /app-data /app/SerilogStorage && chown -R $APP_UID:$APP_UID /app-data /app/SerilogStorage
ENV ASPNETCORE_URLS=http://+:5040
EXPOSE 5040
USER $APP_UID
ENTRYPOINT ["dotnet", "ChatbotConversacional.Storage.Api.dll"]
