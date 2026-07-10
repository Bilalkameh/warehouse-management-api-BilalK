using Warehouse.Application.Commands.Products.CreateProduct;
using Warehouse.Domain.Interfaces;
using Warehouse.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IProductRepository, FakeProductRepository>();
builder.Services.AddSingleton<ISupplierRepository, FakeSupplierRepository>();
builder.Services.AddSingleton<IProductImageRepository, FakeProductImageRepository>();

// mediatr dependencies
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssemblyContaining<CreateProductRequest>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();