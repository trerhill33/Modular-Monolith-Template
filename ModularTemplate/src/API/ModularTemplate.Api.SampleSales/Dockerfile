# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files for restore
COPY ModularTemplate/ModularTemplate.Api.sln ./ModularTemplate/
COPY ModularTemplate/src/API/ModularTemplate.Api.Shared/*.csproj ./ModularTemplate/src/API/ModularTemplate.Api.Shared/
COPY ModularTemplate/src/API/ModularTemplate.Api.SampleSales/*.csproj ./ModularTemplate/src/API/ModularTemplate.Api.SampleSales/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Domain/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Domain/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Application/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Application/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Infrastructure/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Infrastructure/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Presentation/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Presentation/
COPY ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.Domain/*.csproj ./ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.Domain/
COPY ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.Application/*.csproj ./ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.Application/
COPY ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.Infrastructure/*.csproj ./ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.Infrastructure/
COPY ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.Presentation/*.csproj ./ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.Presentation/
COPY ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.IntegrationEvents/*.csproj ./ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.IntegrationEvents/
COPY ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.IntegrationEvents/*.csproj ./ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.IntegrationEvents/

# Restore dependencies
RUN dotnet restore ModularTemplate/src/API/ModularTemplate.Api.SampleSales/ModularTemplate.Api.SampleSales.csproj

# Copy everything else
COPY . .

# Build and publish
RUN dotnet publish ModularTemplate/src/API/ModularTemplate.Api.SampleSales/ModularTemplate.Api.SampleSales.csproj \
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

ENTRYPOINT ["dotnet", "ModularTemplate.Api.SampleSales.dll"]
