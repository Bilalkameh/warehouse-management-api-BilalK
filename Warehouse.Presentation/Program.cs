using Warehouse.Application.Commands.Products.CreateProduct;
using Warehouse.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Warehouse.Infrastructure.Persistence;
using Warehouse.Application.Mappings;
using Warehouse.Presentation.Middleware;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Presentation.Filters;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WarehouseDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"));
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
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(
    configuration => { }, typeof(MappingProfile));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();

// mediatr dependencies
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssemblyContaining<CreateProductRequest>();
});
    
var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
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