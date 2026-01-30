# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files for restore
COPY rtl-core-api/Rtl.Core.Api.sln ./rtl-core-api/
COPY rtl-core-api/src/Api/Shared/*.csproj ./rtl-core-api/src/Api/Shared/
COPY rtl-core-api/src/API/Rtl.Core.Api.SampleOrders/*.csproj ./rtl-core-api/src/API/Rtl.Core.Api.SampleOrders/
COPY rtl-core-api/src/Common/Domain/*.csproj ./rtl-core-api/src/Common/Domain/
COPY rtl-core-api/src/Common/Application/*.csproj ./rtl-core-api/src/Common/Application/
COPY rtl-core-api/src/Common/Infrastructure/*.csproj ./rtl-core-api/src/Common/Infrastructure/
COPY rtl-core-api/src/Common/Presentation/*.csproj ./rtl-core-api/src/Common/Presentation/
COPY rtl-core-api/src/Modules/SampleOrders/Domain/*.csproj ./rtl-core-api/src/Modules/SampleOrders/Domain/
COPY rtl-core-api/src/Modules/SampleOrders/Application/*.csproj ./rtl-core-api/src/Modules/SampleOrders/Application/
COPY rtl-core-api/src/Modules/SampleOrders/Infrastructure/*.csproj ./rtl-core-api/src/Modules/SampleOrders/Infrastructure/
COPY rtl-core-api/src/Modules/SampleOrders/Presentation/*.csproj ./rtl-core-api/src/Modules/SampleOrders/Presentation/
COPY rtl-core-api/src/Modules/SampleOrders/IntegrationEvents/*.csproj ./rtl-core-api/src/Modules/SampleOrders/IntegrationEvents/
COPY rtl-core-api/src/Modules/SampleSales/IntegrationEvents/*.csproj ./rtl-core-api/src/Modules/SampleSales/IntegrationEvents/

# Restore dependencies
RUN dotnet restore rtl-core-api/src/API/Rtl.Core.Api.SampleOrders/Rtl.Core.Api.SampleOrders.csproj

# Copy everything else
COPY . .

# Build and publish
RUN dotnet publish rtl-core-api/src/API/Rtl.Core.Api.SampleOrders/Rtl.Core.Api.SampleOrders.csproj \
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

ENTRYPOINT ["dotnet", "Rtl.Core.Api.SampleOrders.dll"]
