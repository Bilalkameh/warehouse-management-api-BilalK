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


var builder = WebApplication.CreateBuilder(args);

var logPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "Logs",
    "warehouse-log-.txt");

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
});

builder.Services.AddAutoMapper(
    configuration => { }, typeof(MappingProfile));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IInventoryDashboardRepository, InventoryDashboardRepository>();
builder.Services.AddScoped<IValidator<StockAdjustmentRequest>, StockAdjustmentRequestValidator>();

// mediatr dependencies
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssemblyContaining<CreateProductRequest>();
});
    
//Localization
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

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

//Caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration.GetConnectionString("Redis");

    options.InstanceName = "Warehouse_";
});

builder.Services.AddScoped<ICacheService, RedisCacheService>();


//Health checks
builder.Services
    .AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "PostgreSQL")
    .AddCheck<RedisRetryHealthCheck>("Redis");

builder.Services
    .AddHealthChecksUI(options =>
    {
        options.AddHealthCheckEndpoint("Warehouse API", "/health");
    })
    .AddInMemoryStorage();


// Background jobs
builder.Services.AddHangfire(configuration =>
{
    configuration.UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(
            builder.Configuration.GetConnectionString(
                "DefaultConnection"));
    });
});

builder.Services.AddHangfireServer();
builder.Services.AddScoped<ProductExpiryJob>();



var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseRequestLocalization();
app.UseMiddleware<RequestTimingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard("/hangfire");
}

app.UseHttpsRedirection();

app.MapControllers();

//Map health checks
app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
    options.ApiPath = "/health-ui-api";
});



var productExpirySchedule =
    app.Configuration["BackgroundJobs:ProductExpirySchedule"] ?? Cron.Daily();

RecurringJob.AddOrUpdate<ProductExpiryJob>("product-expiry-check",
    job => job.CheckProductExpiryAsync(CancellationToken.None), 
    productExpirySchedule);

app.Run();