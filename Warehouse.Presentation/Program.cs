using Warehouse.Application.Commands.Products.CreateProduct;
using Warehouse.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Warehouse.Infrastructure.Persistence;
using Warehouse.Application.Mappings;
using Warehouse.Presentation.Middleware;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Presentation.Filters;
using FluentValidation;
using Warehouse.Application.Commands.Stock;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Warehouse.Presentation.Swagger;
using Warehouse.Application.Interfaces;
using Warehouse.Infrastructure.Cache;
using Serilog;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Hangfire;
using Hangfire.PostgreSql;
using Warehouse.Application.BackgroundJobs;
using Warehouse.Presentation.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Warehouse.Presentation.Authorization;
using Microsoft.OpenApi.Models;
using Warehouse.Application.Services;
using Warehouse.Infrastructure;
using Warehouse.Presentation.Services;
using Warehouse.Application.Settings;


var builder = WebApplication.CreateBuilder(args);

var logPath = Path.Combine(builder.Environment.ContentRootPath, "Logs", "warehouse-log-.txt");

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File(logPath, rollingInterval: RollingInterval.Day);
});

builder.Services.AddDbContextFactory<WarehouseDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ActionLoggingFilter>();
builder.Services.AddScoped<ModelValidationFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ActionLoggingFilter>();
    options.Filters.AddService<ModelValidationFilter>();
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<AcceptLanguageHeaderOperationFilter>();

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your Firebase ID token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAutoMapper(configuration => { }, typeof(MappingProfile));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<ISupplierDocumentRepository, SupplierDocumentRepository>();
builder.Services.AddScoped<IInventoryDashboardRepository, InventoryDashboardRepository>();
builder.Services.AddScoped<IValidator<StockAdjustmentRequest>, StockAdjustmentRequestValidator>();

// MediatR dependencies
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssemblyContaining<CreateProductRequest>();
});



// Localization
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddMinioStorage(builder.Configuration);
}
builder.Services.AddRabbitMqMessaging(builder.Configuration);

builder.Services.Configure<LowStockSettings>(builder.Configuration.GetSection(LowStockSettings.SectionName));
builder.Services.AddScoped<LowStockEventService>();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("fr-FR")
    };

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Warehouse_";
});

builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Health checks
builder.Services
    .AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "PostgreSQL")
    .AddCheck<RedisRetryHealthCheck>("Redis")
    .AddCheck<MinioHealthCheck>("MinIO");


builder.Services
    .AddHealthChecksUI(options =>
    {
        options.AddHealthCheckEndpoint("Warehouse API", "/health");
    })
    .AddInMemoryStorage();

// Background jobs
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddHangfire(configuration =>
    {
        configuration.UsePostgreSqlStorage(options =>
        {
            options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
    });
    builder.Services.AddHangfireServer();
}

builder.Services.AddScoped<ProductExpiryJob>();



// Firebase authentication
var firebaseProjectId = builder.Configuration["Firebase:ProjectId"]
                        ?? throw new InvalidOperationException("Firebase ProjectId is not configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{firebaseProjectId}";

        options.Audience = firebaseProjectId;

        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{firebaseProjectId}",

            ValidateAudience = true,
            ValidAudience = firebaseProjectId,
            ValidateLifetime = true,
            RoleClaimType = "role",
            NameClaimType = "email"
        };
    });


// Authorization 

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.Admin,policy => policy.RequireRole("admin"));

    options.AddPolicy(AuthorizationPolicies.User,policy => policy.RequireRole("admin", "user"));
});

builder.Services.AddHttpClient();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICorrelationIdAccessor, HttpContextCorrelationIdAccessor>();


var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseRequestLocalization();
app.UseMiddleware<RequestTimingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment() ||
    app.Environment.IsEnvironment("Testing"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire");
}

app.UseHttpsRedirection();

// authentication happens before authorization
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// map healthChecks
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
    options.ApiPath = "/health-ui-api";
});

if (!app.Environment.IsEnvironment("Testing"))
{
    var productExpirySchedule = app.Configuration["BackgroundJobs:ProductExpirySchedule"] ?? Cron.Daily();

    RecurringJob.AddOrUpdate<ProductExpiryJob>("product-expiry-check",
        job => job.CheckProductExpiryAsync(CancellationToken.None),
        productExpirySchedule);
}

app.Run();

public partial class Program
{
}