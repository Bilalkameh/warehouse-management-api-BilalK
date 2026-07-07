using warehouse_management;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<FakeWarehouseStore>();
builder.Services.AddSingleton<FakeSupplierStore>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<SupplierService>();       


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();