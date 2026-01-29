# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files for restore
COPY ModularTemplate/ModularTemplate.Api.sln ./ModularTemplate/
COPY ModularTemplate/src/API/ModularTemplate.Api/*.csproj ./ModularTemplate/src/API/ModularTemplate.Api/
COPY ModularTemplate/src/API/ModularTemplate.Api.Shared/*.csproj ./ModularTemplate/src/API/ModularTemplate.Api.Shared/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Domain/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Domain/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Application/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Application/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Infrastructure/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Infrastructure/
COPY ModularTemplate/src/Common/ModularTemplate.Common.Presentation/*.csproj ./ModularTemplate/src/Common/ModularTemplate.Common.Presentation/
COPY ModularTemplate/src/Modules/Fees/ModularTemplate.Modules.Fees.Domain/*.csproj ./ModularTemplate/src/Modules/Fees/ModularTemplate.Modules.Fees.Domain/
COPY ModularTemplate/src/Modules/Fees/ModularTemplate.Modules.Fees.Application/*.csproj ./ModularTemplate/src/Modules/Fees/ModularTemplate.Modules.Fees.Application/
COPY ModularTemplate/src/Modules/Fees/ModularTemplate.Modules.Fees.Infrastructure/*.csproj ./ModularTemplate/src/Modules/Fees/ModularTemplate.Modules.Fees.Infrastructure/
COPY ModularTemplate/src/Modules/Fees/ModularTemplate.Modules.Fees.Presentation/*.csproj ./ModularTemplate/src/Modules/Fees/ModularTemplate.Modules.Fees.Presentation/
COPY ModularTemplate/src/Modules/Fees/ModularTemplate.Modules.Fees.IntegrationEvents/*.csproj ./ModularTemplate/src/Modules/Fees/ModularTemplate.Modules.Fees.IntegrationEvents/
COPY ModularTemplate/src/Modules/Product/ModularTemplate.Modules.Product.Domain/*.csproj ./ModularTemplate/src/Modules/Product/ModularTemplate.Modules.Product.Domain/
COPY ModularTemplate/src/Modules/Product/ModularTemplate.Modules.Product.Application/*.csproj ./ModularTemplate/src/Modules/Product/ModularTemplate.Modules.Product.Application/
COPY ModularTemplate/src/Modules/Product/ModularTemplate.Modules.Product.Infrastructure/*.csproj ./ModularTemplate/src/Modules/Product/ModularTemplate.Modules.Product.Infrastructure/
COPY ModularTemplate/src/Modules/Product/ModularTemplate.Modules.Product.Presentation/*.csproj ./ModularTemplate/src/Modules/Product/ModularTemplate.Modules.Product.Presentation/
COPY ModularTemplate/src/Modules/Product/ModularTemplate.Modules.Product.IntegrationEvents/*.csproj ./ModularTemplate/src/Modules/Product/ModularTemplate.Modules.Product.IntegrationEvents/
COPY ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.Domain/*.csproj ./ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.Domain/
COPY ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.Application/*.csproj ./ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.Application/
COPY ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.Infrastructure/*.csproj ./ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.Infrastructure/
COPY ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.Presentation/*.csproj ./ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.Presentation/
COPY ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.IntegrationEvents/*.csproj ./ModularTemplate/src/Modules/SampleOrders/ModularTemplate.Modules.SampleOrders.IntegrationEvents/
COPY ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.Domain/*.csproj ./ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.Domain/
COPY ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.Application/*.csproj ./ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.Application/
COPY ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.Infrastructure/*.csproj ./ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.Infrastructure/
COPY ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.Presentation/*.csproj ./ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.Presentation/
COPY ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.IntegrationEvents/*.csproj ./ModularTemplate/src/Modules/SampleSales/ModularTemplate.Modules.SampleSales.IntegrationEvents/
COPY ModularTemplate/src/Modules/Organization/ModularTemplate.Modules.Organization.Domain/*.csproj ./ModularTemplate/src/Modules/Organization/ModularTemplate.Modules.Organization.Domain/
COPY ModularTemplate/src/Modules/Organization/ModularTemplate.Modules.Organization.Application/*.csproj ./ModularTemplate/src/Modules/Organization/ModularTemplate.Modules.Organization.Application/
COPY ModularTemplate/src/Modules/Organization/ModularTemplate.Modules.Organization.Infrastructure/*.csproj ./ModularTemplate/src/Modules/Organization/ModularTemplate.Modules.Organization.Infrastructure/
COPY ModularTemplate/src/Modules/Organization/ModularTemplate.Modules.Organization.Presentation/*.csproj ./ModularTemplate/src/Modules/Organization/ModularTemplate.Modules.Organization.Presentation/
COPY ModularTemplate/src/Modules/Organization/ModularTemplate.Modules.Organization.IntegrationEvents/*.csproj ./ModularTemplate/src/Modules/Organization/ModularTemplate.Modules.Organization.IntegrationEvents/
COPY ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.Domain/*.csproj ./ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.Domain/
COPY ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.Application/*.csproj ./ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.Application/
COPY ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.Infrastructure/*.csproj ./ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.Infrastructure/
COPY ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.Presentation/*.csproj ./ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.Presentation/
COPY ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.IntegrationEvents/*.csproj ./ModularTemplate/src/Modules/Customer/ModularTemplate.Modules.Customer.IntegrationEvents/
COPY ModularTemplate/src/Modules/Sales/ModularTemplate.Modules.Sales.Domain/*.csproj ./ModularTemplate/src/Modules/Sales/ModularTemplate.Modules.Sales.Domain/
COPY ModularTemplate/src/Modules/Sales/ModularTemplate.Modules.Sales.Application/*.csproj ./ModularTemplate/src/Modules/Sales/ModularTemplate.Modules.Sales.Application/
COPY ModularTemplate/src/Modules/Sales/ModularTemplate.Modules.Sales.Infrastructure/*.csproj ./ModularTemplate/src/Modules/Sales/ModularTemplate.Modules.Sales.Infrastructure/
COPY ModularTemplate/src/Modules/Sales/ModularTemplate.Modules.Sales.Presentation/*.csproj ./ModularTemplate/src/Modules/Sales/ModularTemplate.Modules.Sales.Presentation/
COPY ModularTemplate/src/Modules/Sales/ModularTemplate.Modules.Sales.IntegrationEvents/*.csproj ./ModularTemplate/src/Modules/Sales/ModularTemplate.Modules.Sales.IntegrationEvents/

# Restore dependencies
RUN dotnet restore ModularTemplate/src/API/ModularTemplate.Api/ModularTemplate.Api.csproj

# Copy everything else
COPY . .

# Build and publish
RUN dotnet publish ModularTemplate/src/API/ModularTemplate.Api/ModularTemplate.Api.csproj \
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

ENTRYPOINT ["dotnet", "ModularTemplate.Api.dll"]
