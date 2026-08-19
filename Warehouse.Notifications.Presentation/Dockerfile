FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first so Docker can cache NuGet restore separately.
COPY ["Warehouse.Notifications.Domain/Warehouse.Notifications.Domain.csproj", "Warehouse.Notifications.Domain/"]
COPY ["Warehouse.Notifications.Application/Warehouse.Notifications.Application.csproj", "Warehouse.Notifications.Application/"]
COPY ["Warehouse.Notifications.Infrastructure/Warehouse.Notifications.Infrastructure.csproj", "Warehouse.Notifications.Infrastructure/"]
COPY ["Warehouse.Notifications.Presentation/Warehouse.Notifications.Presentation.csproj", "Warehouse.Notifications.Presentation/"]

RUN dotnet restore "Warehouse.Notifications.Presentation/Warehouse.Notifications.Presentation.csproj"

COPY . .
WORKDIR /src/Warehouse.Notifications.Presentation
RUN dotnet publish "Warehouse.Notifications.Presentation.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .

RUN chown -R app:app /app
USER app

ENTRYPOINT ["dotnet", "Warehouse.Notifications.Presentation.dll"]

