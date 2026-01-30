# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files for restore
COPY rtl-core-api/Rtl.Core.Api.sln ./rtl-core-api/
COPY rtl-core-api/src/Api/Host/*.csproj ./rtl-core-api/src/Api/Host/
COPY rtl-core-api/src/Api/Shared/*.csproj ./rtl-core-api/src/Api/Shared/
COPY rtl-core-api/src/Common/Domain/*.csproj ./rtl-core-api/src/Common/Domain/
COPY rtl-core-api/src/Common/Application/*.csproj ./rtl-core-api/src/Common/Application/
COPY rtl-core-api/src/Common/Infrastructure/*.csproj ./rtl-core-api/src/Common/Infrastructure/
COPY rtl-core-api/src/Common/Presentation/*.csproj ./rtl-core-api/src/Common/Presentation/
COPY rtl-core-api/src/Modules/Fees/Domain/*.csproj ./rtl-core-api/src/Modules/Fees/Domain/
COPY rtl-core-api/src/Modules/Fees/Application/*.csproj ./rtl-core-api/src/Modules/Fees/Application/
COPY rtl-core-api/src/Modules/Fees/Infrastructure/*.csproj ./rtl-core-api/src/Modules/Fees/Infrastructure/
COPY rtl-core-api/src/Modules/Fees/Presentation/*.csproj ./rtl-core-api/src/Modules/Fees/Presentation/
COPY rtl-core-api/src/Modules/Fees/IntegrationEvents/*.csproj ./rtl-core-api/src/Modules/Fees/IntegrationEvents/
COPY rtl-core-api/src/Modules/Product/Domain/*.csproj ./rtl-core-api/src/Modules/Product/Domain/
COPY rtl-core-api/src/Modules/Product/Application/*.csproj ./rtl-core-api/src/Modules/Product/Application/
COPY rtl-core-api/src/Modules/Product/Infrastructure/*.csproj ./rtl-core-api/src/Modules/Product/Infrastructure/
COPY rtl-core-api/src/Modules/Product/Presentation/*.csproj ./rtl-core-api/src/Modules/Product/Presentation/
COPY rtl-core-api/src/Modules/Product/IntegrationEvents/*.csproj ./rtl-core-api/src/Modules/Product/IntegrationEvents/
COPY rtl-core-api/src/Modules/SampleOrders/Domain/*.csproj ./rtl-core-api/src/Modules/SampleOrders/Domain/
COPY rtl-core-api/src/Modules/SampleOrders/Application/*.csproj ./rtl-core-api/src/Modules/SampleOrders/Application/
COPY rtl-core-api/src/Modules/SampleOrders/Infrastructure/*.csproj ./rtl-core-api/src/Modules/SampleOrders/Infrastructure/
COPY rtl-core-api/src/Modules/SampleOrders/Presentation/*.csproj ./rtl-core-api/src/Modules/SampleOrders/Presentation/
COPY rtl-core-api/src/Modules/SampleOrders/IntegrationEvents/*.csproj ./rtl-core-api/src/Modules/SampleOrders/IntegrationEvents/
COPY rtl-core-api/src/Modules/SampleSales/Domain/*.csproj ./rtl-core-api/src/Modules/SampleSales/Domain/
COPY rtl-core-api/src/Modules/SampleSales/Application/*.csproj ./rtl-core-api/src/Modules/SampleSales/Application/
COPY rtl-core-api/src/Modules/SampleSales/Infrastructure/*.csproj ./rtl-core-api/src/Modules/SampleSales/Infrastructure/
COPY rtl-core-api/src/Modules/SampleSales/Presentation/*.csproj ./rtl-core-api/src/Modules/SampleSales/Presentation/
COPY rtl-core-api/src/Modules/SampleSales/IntegrationEvents/*.csproj ./rtl-core-api/src/Modules/SampleSales/IntegrationEvents/
COPY rtl-core-api/src/Modules/Organization/Domain/*.csproj ./rtl-core-api/src/Modules/Organization/Domain/
COPY rtl-core-api/src/Modules/Organization/Application/*.csproj ./rtl-core-api/src/Modules/Organization/Application/
COPY rtl-core-api/src/Modules/Organization/Infrastructure/*.csproj ./rtl-core-api/src/Modules/Organization/Infrastructure/
COPY rtl-core-api/src/Modules/Organization/Presentation/*.csproj ./rtl-core-api/src/Modules/Organization/Presentation/
COPY rtl-core-api/src/Modules/Organization/IntegrationEvents/*.csproj ./rtl-core-api/src/Modules/Organization/IntegrationEvents/
COPY rtl-core-api/src/Modules/Customer/Domain/*.csproj ./rtl-core-api/src/Modules/Customer/Domain/
COPY rtl-core-api/src/Modules/Customer/Application/*.csproj ./rtl-core-api/src/Modules/Customer/Application/
COPY rtl-core-api/src/Modules/Customer/Infrastructure/*.csproj ./rtl-core-api/src/Modules/Customer/Infrastructure/
COPY rtl-core-api/src/Modules/Customer/Presentation/*.csproj ./rtl-core-api/src/Modules/Customer/Presentation/
COPY rtl-core-api/src/Modules/Customer/IntegrationEvents/*.csproj ./rtl-core-api/src/Modules/Customer/IntegrationEvents/
COPY rtl-core-api/src/Modules/Sales/Domain/*.csproj ./rtl-core-api/src/Modules/Sales/Domain/
COPY rtl-core-api/src/Modules/Sales/Application/*.csproj ./rtl-core-api/src/Modules/Sales/Application/
COPY rtl-core-api/src/Modules/Sales/Infrastructure/*.csproj ./rtl-core-api/src/Modules/Sales/Infrastructure/
COPY rtl-core-api/src/Modules/Sales/Presentation/*.csproj ./rtl-core-api/src/Modules/Sales/Presentation/
COPY rtl-core-api/src/Modules/Sales/IntegrationEvents/*.csproj ./rtl-core-api/src/Modules/Sales/IntegrationEvents/

# Restore dependencies
RUN dotnet restore rtl-core-api/src/Api/Host/Rtl.Core.Api.csproj

# Copy everything else
COPY . .

# Build and publish
RUN dotnet publish rtl-core-api/src/Api/Host/Rtl.Core.Api.csproj \
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

ENTRYPOINT ["dotnet", "Rtl.Core.Api.dll"]
