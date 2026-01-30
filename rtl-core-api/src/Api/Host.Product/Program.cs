using Asp.Versioning;
using Rtl.Core.Api.Shared;
using Rtl.Core.Application;
using Rtl.Core.Infrastructure;
using Rtl.Module.Product.Infrastructure;
using Rtl.Module.Product.Infrastructure.Persistence;
using Rtl.Module.Product.Presentation.Endpoints;
using ProductApplication = Rtl.Module.Product.Application.AssemblyReference;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// Configuration
// ========================================

var databaseConnectionString = builder.Configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException("Database connection string is required");

var cacheConnectionString = builder.Configuration.GetConnectionString("Cache")
    ?? "localhost:6379";

// ========================================
// Service Configuration
// ========================================

var moduleEndpoints = new ProductModuleEndpoints();

// Add shared host defaults (health checks, versioning, OpenAPI, exception handling)
builder.AddModuleHostDefaults(
    "Product",
    databaseConnectionString,
    cacheConnectionString,
    moduleEndpoints);

// Application layer (MediatR, FluentValidation, Pipeline Behaviors)
builder.Services.AddCommonApplication([ProductApplication.Assembly]);

// Infrastructure layer (Database, Cache, Auth, Workers, Messaging)
builder.Services.AddCommonInfrastructure(
    builder.Configuration,
    builder.Environment,
    databaseConnectionString,
    cacheConnectionString);

// Product module services
builder.Services.AddProductModule(
    builder.Configuration,
    builder.Environment,
    databaseConnectionString);

// ========================================
// Middleware Pipeline
// ========================================

var app = builder.Build();

// Apply migrations in development
app.ApplyMigrations<ProductDbContext>(builder.Environment, databaseConnectionString);

// Create the API version set for endpoint mapping
var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .HasApiVersion(new ApiVersion(2, 0))
    .ReportApiVersions()
    .Build();

// Apply shared middleware
app.UseModuleHostDefaults(moduleEndpoints);

// Map module endpoints
moduleEndpoints.MapEndpoints(app, apiVersionSet);

app.Run();

// Expose Program class for integration tests
public partial class Program;
