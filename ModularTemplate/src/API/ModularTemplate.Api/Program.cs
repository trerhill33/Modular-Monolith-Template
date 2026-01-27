using ModularTemplate.Api.Extensions;
using ModularTemplate.Api.Shared;
using ModularTemplate.Common.Application;
using ModularTemplate.Common.Infrastructure;
using ModularTemplate.Modules.Customer.Infrastructure;
using ModularTemplate.Modules.Customer.Infrastructure.Persistence;
using ModularTemplate.Modules.Orders.Infrastructure;
using ModularTemplate.Modules.Orders.Infrastructure.Persistence;
using ModularTemplate.Modules.Organization.Infrastructure;
using ModularTemplate.Modules.Organization.Infrastructure.Persistence;
using ModularTemplate.Modules.Sales.Infrastructure;
using ModularTemplate.Modules.Sales.Infrastructure.Persistence;
using ModularTemplate.Modules.Sample.Infrastructure;
using ModularTemplate.Modules.Sample.Infrastructure.Persistence;
using CustomerApplication = ModularTemplate.Modules.Customer.Application.AssemblyReference;
using OrdersApplication = ModularTemplate.Modules.Orders.Application.AssemblyReference;
using OrganizationApplication = ModularTemplate.Modules.Organization.Application.AssemblyReference;
using SalesApplication = ModularTemplate.Modules.Sales.Application.AssemblyReference;
using SampleApplication = ModularTemplate.Modules.Sample.Application.AssemblyReference;

var builder = WebApplication.CreateBuilder(args);
var modules = ModuleExtensions.GetModuleEndpoints();

// ========================================
// Service Configuration
// ========================================

var databaseConnectionString = builder.Configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException("Database connection string is required");

var cacheConnectionString = builder.Configuration.GetConnectionString("Cache")
    ?? "localhost:6379";

// Load module-specific configuration files
builder.Configuration.AddModuleConfiguration(["sample", "orders", "organization", "customer", "sales"], builder.Environment.EnvironmentName);

// ========================================
// Common Cross-Cutting Concerns
// ========================================

// Presentation/API layer
builder.Services
    .AddGlobalExceptionHandling()
    .AddApiVersioningServices()
    .AddOpenApiVersioned(builder.Configuration["Application:DisplayName"] ?? "API", modules)
    .AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    })
    .AddHealthChecks(databaseConnectionString, cacheConnectionString);

// Application layer (MediatR, FluentValidation, Pipeline Behaviors)
builder.Services.AddCommonApplication([
    SampleApplication.Assembly,
    OrdersApplication.Assembly,
    OrganizationApplication.Assembly,
    CustomerApplication.Assembly,
    SalesApplication.Assembly]);

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
    .AddSampleModule(builder.Configuration, builder.Environment, DatabaseMigrationExtensions.GetModuleConnectionString(builder.Configuration, "Sample", databaseConnectionString))
    .AddOrdersModule(builder.Configuration, builder.Environment, DatabaseMigrationExtensions.GetModuleConnectionString(builder.Configuration, "Orders", databaseConnectionString))
    .AddOrganizationModule(builder.Configuration, builder.Environment, DatabaseMigrationExtensions.GetModuleConnectionString(builder.Configuration, "Organization", databaseConnectionString))
    .AddCustomerModule(builder.Configuration, builder.Environment, DatabaseMigrationExtensions.GetModuleConnectionString(builder.Configuration, "Customer", databaseConnectionString))
    .AddSalesModule(builder.Configuration, builder.Environment, DatabaseMigrationExtensions.GetModuleConnectionString(builder.Configuration, "Sales", databaseConnectionString));

// ========================================
// Middleware Pipeline
// ========================================

var app = builder.Build();

// Apply migrations for all modules (supports multi-database when configured)
app.ApplyMigrations(
    builder.Environment,
    builder.Configuration,
    databaseConnectionString,
    ("Sample", typeof(SampleDbContext)),
    ("Orders", typeof(OrdersDbContext)),
    ("Organization", typeof(OrganizationDbContext)),
    ("Customer", typeof(CustomerDbContext)),
    ("Sales", typeof(SalesDbContext)));

// Create the API version set for endpoint mapping
var apiVersionSet = app.CreateApiVersionSet();

app.UseOpenApiVersioned(modules);
app.MapHealthCheckEndpoint();
app.UseGlobalExceptionHandling();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Map versioned module endpoints
app.MapVersionedModuleEndpoints(apiVersionSet, modules);

app.Run();

// Expose Program class for integration tests
public partial class Program;
