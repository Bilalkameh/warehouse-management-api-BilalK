FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first so Docker can cache NuGet restore separately.
COPY ["Warehouse.Domain/Warehouse.Domain.csproj", "Warehouse.Domain/"]
COPY ["Warehouse.Application/Warehouse.Application.csproj", "Warehouse.Application/"]
COPY ["Warehouse.Infrastructure/Warehouse.Infrastructure.csproj", "Warehouse.Infrastructure/"]
COPY ["Warehouse.Presentation/Warehouse.Presentation.csproj", "Warehouse.Presentation/"]

RUN dotnet restore "Warehouse.Presentation/Warehouse.Presentation.csproj"

COPY . .
WORKDIR /src/Warehouse.Presentation
RUN dotnet publish "Warehouse.Presentation.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# .NET 8 container images use the non-privileged HTTP port 8080.
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .

RUN mkdir -p /app/Logs && chown -R app:app /app
USER app

ENTRYPOINT ["dotnet", "Warehouse.Presentation.dll"]

