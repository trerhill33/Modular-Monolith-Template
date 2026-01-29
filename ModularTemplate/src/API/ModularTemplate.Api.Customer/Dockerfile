# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files for restore
COPY ModularTemplate/ModularTemplate.Api.sln ./ModularTemplate/
COPY ModularTemplate/src/API/ModularTemplate.Api.Shared/*.csproj ./ModularTemplate/src/API/ModularTemplate.Api.Shared/
COPY ModularTemplate/src/API/ModularTemplate.Api.Customer/*.csproj ./ModularTemplate/src/API/ModularTemplate.Api.Customer/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Domain/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Domain/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Application/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Application/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Infrastructure/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Infrastructure/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Presentation/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Presentation/
COPY ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.Domain/*.csproj ./ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.Domain/
COPY ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.Application/*.csproj ./ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.Application/
COPY ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.Infrastructure/*.csproj ./ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.Infrastructure/
COPY ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.Presentation/*.csproj ./ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.Presentation/
COPY ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.IntegrationEvents/*.csproj ./ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.IntegrationEvents/

# Restore dependencies
RUN dotnet restore ModularTemplate/src/API/ModularTemplate.Api.Customer/ModularTemplate.Api.Customer.csproj

# Copy everything else
COPY . .

# Build and publish
RUN dotnet publish ModularTemplate/src/API/ModularTemplate.Api.Customer/ModularTemplate.Api.Customer.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
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

ENTRYPOINT ["dotnet", "ModularTemplate.Api.Customer.dll"]
