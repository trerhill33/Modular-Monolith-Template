# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files for restore
COPY ModularTemplate/ModularTemplate.Api.sln ./ModularTemplate/
COPY ModularTemplate/src/API/ModularTemplate.Api.Host/*.csproj ./ModularTemplate/src/API/ModularTemplate.Api.Host/
COPY ModularTemplate/src/API/ModularTemplate.Api.Orders/*.csproj ./ModularTemplate/src/API/ModularTemplate.Api.Orders/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Domain/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Domain/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Application/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Application/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Infrastructure/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Infrastructure/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Presentation/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Presentation/
COPY ModularTemplate/src/Modules/Orders/ModularTemplate.Modules.Orders.Domain/*.csproj ./ModularTemplate/src/Modules/Orders/ModularTemplate.Modules.Orders.Domain/
COPY ModularTemplate/src/Modules/Orders/ModularTemplate.Modules.Orders.Application/*.csproj ./ModularTemplate/src/Modules/Orders/ModularTemplate.Modules.Orders.Application/
COPY ModularTemplate/src/Modules/Orders/ModularTemplate.Modules.Orders.Infrastructure/*.csproj ./ModularTemplate/src/Modules/Orders/ModularTemplate.Modules.Orders.Infrastructure/
COPY ModularTemplate/src/Modules/Orders/ModularTemplate.Modules.Orders.Presentation/*.csproj ./ModularTemplate/src/Modules/Orders/ModularTemplate.Modules.Orders.Presentation/
COPY ModularTemplate/src/Modules/Orders/ModularTemplate.Modules.Orders.IntegrationEvents/*.csproj ./ModularTemplate/src/Modules/Orders/ModularTemplate.Modules.Orders.IntegrationEvents/

# Restore dependencies
RUN dotnet restore ModularTemplate/src/API/ModularTemplate.Api.Orders/ModularTemplate.Api.Orders.csproj

# Copy everything else
COPY . .

# Build and publish
RUN dotnet publish ModularTemplate/src/API/ModularTemplate.Api.Orders/ModularTemplate.Api.Orders.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copy published app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=5s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ModularTemplate.Api.Orders.dll"]
