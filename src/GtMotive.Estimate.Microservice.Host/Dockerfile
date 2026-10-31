FROM mcr.microsoft.com/dotnet/sdk:9.0.200 AS build
WORKDIR /repo
COPY . .
RUN dotnet publish src/GtMotive.Estimate.Microservice.Host/GtMotive.Estimate.Microservice.Host.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app .
ENTRYPOINT ["dotnet", "GtMotive.Estimate.Microservice.Host.dll"]
