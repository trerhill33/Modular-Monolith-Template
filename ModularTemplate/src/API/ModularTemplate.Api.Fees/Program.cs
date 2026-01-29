using Asp.Versioning;
using ModularTemplate.Api.Shared;
using ModularTemplate.Common.Application;
using ModularTemplate.Common.Infrastructure;
using ModularTemplate.Modules.Fees.Infrastructure;
using ModularTemplate.Modules.Fees.Infrastructure.Persistence;
using ModularTemplate.Modules.Fees.Presentation.Endpoints;
using FeesApplication = ModularTemplate.Modules.Fees.Application.AssemblyReference;

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

var moduleEndpoints = new FeesModuleEndpoints();

// Add shared host defaults (health checks, versioning, OpenAPI, exception handling)
builder.AddModuleHostDefaults(
    "Fees",
    databaseConnectionString,
    cacheConnectionString,
    moduleEndpoints);

// Application layer (MediatR, FluentValidation, Pipeline Behaviors)
builder.Services.AddCommonApplication([FeesApplication.Assembly]);

// Infrastructure layer (Database, Cache, Auth, Workers, Messaging)
builder.Services.AddCommonInfrastructure(
    builder.Configuration,
    builder.Environment,
    databaseConnectionString,
    cacheConnectionString);

// Fees module services
builder.Services.AddFeesModule(
    builder.Configuration,
    builder.Environment,
    databaseConnectionString);

// ========================================
// Middleware Pipeline
// ========================================

var app = builder.Build();

// Apply migrations in development
app.ApplyMigrations<FeesDbContext>(builder.Environment, databaseConnectionString);

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
