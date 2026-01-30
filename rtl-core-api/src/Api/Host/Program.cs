using Rtl.Core.Api.Extensions;
using Rtl.Core.Api.Shared;
using Rtl.Core.Application;
using Rtl.Core.Infrastructure;
using Rtl.Core.Infrastructure.Application;
using Rtl.Module.Customer.Infrastructure;
using Rtl.Module.Customer.Infrastructure.Persistence;
using Rtl.Module.Fees.Infrastructure;
using Rtl.Module.Fees.Infrastructure.Persistence;
using Rtl.Module.Organization.Infrastructure;
using Rtl.Module.Organization.Infrastructure.Persistence;
using Rtl.Module.Product.Infrastructure;
using Rtl.Module.Product.Infrastructure.Persistence;
using Rtl.Module.Sales.Infrastructure;
using Rtl.Module.Sales.Infrastructure.Persistence;
using Rtl.Module.SampleOrders.Infrastructure;
using Rtl.Module.SampleOrders.Infrastructure.Persistence;
using Rtl.Module.SampleSales.Infrastructure;
using Rtl.Module.SampleSales.Infrastructure.Persistence;
using Serilog;
using CustomerApplication = Rtl.Module.Customer.Application.AssemblyReference;
using FeesApplication = Rtl.Module.Fees.Application.AssemblyReference;
using OrdersApplication = Rtl.Module.SampleOrders.Application.AssemblyReference;
using OrganizationApplication = Rtl.Module.Organization.Application.AssemblyReference;
using ProductApplication = Rtl.Module.Product.Application.AssemblyReference;
using SalesApplication = Rtl.Module.Sales.Application.AssemblyReference;
using SampleApplication = Rtl.Module.SampleSales.Application.AssemblyReference;

var builder = WebApplication.CreateBuilder(args);
var modules = ModuleExtensions.GetModuleEndpoints();

// ========================================
// Host Configuration
// ========================================

// Configure graceful shutdown timeout for chaos engineering readiness
// Allows time for in-flight requests to complete before forced termination
builder.Host.ConfigureHostOptions(options =>
{
    options.ShutdownTimeout = TimeSpan.FromSeconds(30);
});

// ========================================
// Serilog Configuration
// ========================================
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// ========================================
// Service Configuration
// ========================================

var databaseConnectionString = builder.Configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException("Database connection string is required");

var cacheConnectionString = builder.Configuration.GetConnectionString("Cache")
    ?? "localhost:6379";

// Load module-specific configuration files
builder.Configuration.AddModuleConfiguration(["SampleSales", "SampleOrders", "Organization", "Customer", "Sales", "Fees", "Product"], builder.Environment.EnvironmentName);

// ========================================
// Common Cross-Cutting Concerns
// ========================================

// Application identity - must be registered first as other services depend on it
builder.Services.AddOptions<ApplicationOptions>()
    .Bind(builder.Configuration.GetSection(ApplicationOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Presentation/API layer
builder.Services
    .AddGlobalExceptionHandling()
    .AddApiVersioningServices()
    .AddOpenApiVersioned(builder.Configuration["Application:DisplayName"] ?? "API", modules)
    .AddCorsServices(builder.Configuration, builder.Environment)
    .AddHealthChecks(databaseConnectionString, cacheConnectionString)
    .AddGranularHealthChecks(builder.Configuration)
    .AddRateLimiting(builder.Configuration);

// Application layer (MediatR, FluentValidation, Pipeline Behaviors)
builder.Services.AddCommonApplication([
    SampleApplication.Assembly,
    OrdersApplication.Assembly,
    OrganizationApplication.Assembly,
    CustomerApplication.Assembly,
    SalesApplication.Assembly,
    FeesApplication.Assembly,
    ProductApplication.Assembly]);

// Infrastructure layer (Database, Cache, Auth, Workers, Messaging)
builder.Services.AddCommonInfrastructure(
    builder.Configuration,
    builder.Environment,
    databaseConnectionString,
    cacheConnectionString);

// ========================================
// Module Registrations
// ========================================

builder.Services
    .AddSampleSalesModule(builder.Configuration, builder.Environment, DatabaseMigrationExtensions.GetModuleConnectionString(builder.Configuration, "SampleSales", databaseConnectionString))
    .AddSampleOrdersModule(builder.Configuration, builder.Environment, DatabaseMigrationExtensions.GetModuleConnectionString(builder.Configuration, "SampleOrders", databaseConnectionString))
    .AddOrganizationModule(builder.Configuration, builder.Environment, DatabaseMigrationExtensions.GetModuleConnectionString(builder.Configuration, "Organization", databaseConnectionString))
    .AddCustomerModule(builder.Configuration, builder.Environment, DatabaseMigrationExtensions.GetModuleConnectionString(builder.Configuration, "Customer", databaseConnectionString))
    .AddSalesModule(builder.Configuration, builder.Environment, DatabaseMigrationExtensions.GetModuleConnectionString(builder.Configuration, "Sales", databaseConnectionString))
    .AddFeesModule(builder.Configuration, builder.Environment, DatabaseMigrationExtensions.GetModuleConnectionString(builder.Configuration, "Fees", databaseConnectionString))
    .AddProductModule(builder.Configuration, builder.Environment, DatabaseMigrationExtensions.GetModuleConnectionString(builder.Configuration, "Product", databaseConnectionString));

// ========================================
// Middleware Pipeline
// ========================================

var app = builder.Build();

// Apply migrations for all modules (supports multi-database when configured)
app.ApplyMigrations(
    builder.Environment,
    builder.Configuration,
    databaseConnectionString,
    ("SampleSales", typeof(SampleDbContext)),
    ("SampleOrders", typeof(OrdersDbContext)),
    ("Organization", typeof(OrganizationDbContext)),
    ("Customer", typeof(CustomerDbContext)),
    ("Sales", typeof(SalesDbContext)),
    ("Fees", typeof(FeesDbContext)),
    ("Product", typeof(ProductDbContext)));

// Create the API version set for endpoint mapping
var apiVersionSet = app.CreateApiVersionSet();

// Serilog request logging
app.UseSerilogRequestLogging();

app.UseOpenApiVersioned(modules);

// Health check endpoints for Kubernetes probes and monitoring
app.MapHealthCheckEndpoint();                                      // /health - full health check
app.MapLivenessProbeEndpoint();                                    // /health/live - minimal, just checks app responds
app.MapReadinessProbeEndpoint();                                   // /health/ready - database + cache connectivity
app.MapStartupProbeEndpoint();                                     // /health/startup - database only
app.MapTaggedHealthCheckEndpoint("/health/messaging", "messaging");
app.MapTaggedHealthCheckEndpoint("/health/modules", "module");

app.UseGlobalExceptionHandling();
app.UseRateLimiter();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Map versioned module endpoints
app.MapVersionedModuleEndpoints(apiVersionSet, modules);

app.Run();

// Expose Program class for integration tests
public partial class Program;
