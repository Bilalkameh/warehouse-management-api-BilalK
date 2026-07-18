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


var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseRequestLocalization();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestTimingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestTimingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();