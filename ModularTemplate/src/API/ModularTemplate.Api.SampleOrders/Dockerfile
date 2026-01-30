# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files for restore
COPY ModularTemplate/ModularTemplate.Api.sln ./ModularTemplate/
COPY ModularTemplate/src/API/ModularTemplate.Api.Shared/*.csproj ./ModularTemplate/src/API/ModularTemplate.Api.Shared/
COPY ModularTemplate/src/API/ModularTemplate.Api.SampleOrders/*.csproj ./ModularTemplate/src/API/ModularTemplate.Api.SampleOrders/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Domain/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Domain/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Application/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Application/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Infrastructure/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Infrastructure/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Presentation/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Presentation/
COPY ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.Domain/*.csproj ./ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.Domain/
COPY ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.Application/*.csproj ./ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.Application/
COPY ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.Infrastructure/*.csproj ./ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.Infrastructure/
COPY ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.Presentation/*.csproj ./ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.Presentation/
COPY ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.IntegrationEvents/*.csproj ./ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.IntegrationEvents/
COPY ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.IntegrationEvents/*.csproj ./ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.IntegrationEvents/

# Restore dependencies
RUN dotnet restore ModularTemplate/src/API/ModularTemplate.Api.SampleOrders/ModularTemplate.Api.SampleOrders.csproj

# Copy everything else
COPY . .

# Build and publish
RUN dotnet publish ModularTemplate/src/API/ModularTemplate.Api.SampleOrders/ModularTemplate.Api.SampleOrders.csproj \
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

ENTRYPOINT ["dotnet", "ModularTemplate.Api.SampleOrders.dll"]
