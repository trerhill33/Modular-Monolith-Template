# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files for restore
COPY rtl-core-api/Rtl.Core.Api.sln ./rtl-core-api/
COPY rtl-core-api/src/Api/Shared/*.csproj ./rtl-core-api/src/Api/Shared/
COPY rtl-core-api/src/API/Rtl.Core.Api.Sales/*.csproj ./rtl-core-api/src/API/Rtl.Core.Api.Sales/
COPY rtl-core-api/src/Common/Domain/*.csproj ./rtl-core-api/src/Common/Domain/
COPY rtl-core-api/src/Common/Application/*.csproj ./rtl-core-api/src/Common/Application/
COPY rtl-core-api/src/Common/Infrastructure/*.csproj ./rtl-core-api/src/Common/Infrastructure/
COPY rtl-core-api/src/Common/Presentation/*.csproj ./rtl-core-api/src/Common/Presentation/
COPY rtl-core-api/src/Modules/Sales/Domain/*.csproj ./rtl-core-api/src/Modules/Sales/Domain/
COPY rtl-core-api/src/Modules/Sales/Application/*.csproj ./rtl-core-api/src/Modules/Sales/Application/
COPY rtl-core-api/src/Modules/Sales/Infrastructure/*.csproj ./rtl-core-api/src/Modules/Sales/Infrastructure/
COPY rtl-core-api/src/Modules/Sales/Presentation/*.csproj ./rtl-core-api/src/Modules/Sales/Presentation/
COPY rtl-core-api/src/Modules/Sales/IntegrationEvents/*.csproj ./rtl-core-api/src/Modules/Sales/IntegrationEvents/

# Restore dependencies
RUN dotnet restore rtl-core-api/src/API/Rtl.Core.Api.Sales/Rtl.Core.Api.Sales.csproj

# Copy everything else
COPY . .

# Build and publish
RUN dotnet publish rtl-core-api/src/API/Rtl.Core.Api.Sales/Rtl.Core.Api.Sales.csproj \
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

ENTRYPOINT ["dotnet", "Rtl.Core.Api.Sales.dll"]
