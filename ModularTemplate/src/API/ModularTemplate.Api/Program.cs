using ModularTemplate.Api.Extensions;
using ModularTemplate.Common.Application;
using ModularTemplate.Common.Infrastructure;
using ModularTemplate.Modules.Orders.Infrastructure;
using ModularTemplate.Modules.Sales.Infrastructure;
using OrdersApplication = ModularTemplate.Modules.Orders.Application.AssemblyReference;
using SalesApplication = ModularTemplate.Modules.Sales.Application.AssemblyReference;

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
builder.Configuration.AddModuleConfiguration(["sales", "orders"], builder.Environment.EnvironmentName);

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
    SalesApplication.Assembly,
    OrdersApplication.Assembly]);

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
    .AddSalesModule(builder.Configuration, builder.Environment, databaseConnectionString)
    .AddOrdersModule(builder.Configuration, builder.Environment, databaseConnectionString);

// ========================================
// Middleware Pipeline
// ========================================

var app = builder.Build();

app.ApplyMigrations(builder.Environment, databaseConnectionString);

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
