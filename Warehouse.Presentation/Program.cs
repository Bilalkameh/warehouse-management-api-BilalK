using Warehouse.Application.Handlers.Products;
using Warehouse.Application.Handlers.Suppliers;
using Warehouse.Domain.Interfaces;
using Warehouse.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


// Add MVC Controllers
builder.Services.AddControllers();


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




// Repositories
builder.Services.AddSingleton<IProductRepository, FakeProductRepository>();
builder.Services.AddSingleton<ISupplierRepository, FakeSupplierRepository>();
builder.Services.AddSingleton<IProductImageRepository, FakeProductImageRepository>();



// Product Handlers
builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<GetProductByIdHandler>();
builder.Services.AddScoped<GetProductsHandler>();
builder.Services.AddScoped<SearchProductsHandler>();
builder.Services.AddScoped<UpdateProductPriceHandler>();
builder.Services.AddScoped<UpdateProductQuantityHandler>();
builder.Services.AddScoped<ArchiveProductHandler>();
builder.Services.AddScoped<AssignSupplierHandler>();
builder.Services.AddScoped<AddProductImageHandler>();


// Supplier Handlers
builder.Services.AddScoped<CreateSupplierHandler>();
builder.Services.AddScoped<GetSupplierByIdHandler>();
builder.Services.AddScoped<GetSuppliersHandler>();
builder.Services.AddScoped<DeactivateSupplierHandler>();



var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
// Enable Controllers
app.MapControllers();
app.Run();