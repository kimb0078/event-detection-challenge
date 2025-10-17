FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY src/EventDetection/EventDetection.csproj ./EventDetection/
COPY src/EventDetection ./EventDetection/
RUN dotnet publish ./EventDetection/EventDetection.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "EventDetection.dll"]
