using System.Text.Json.Serialization;
using Warehouse.Notifications.Application;
using Warehouse.Notifications.Infrastructure;
using Warehouse.Notifications.Presentation.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

builder.Services.AddApplication();

var connectionString = builder.Configuration.GetConnectionString("NotificationsConnection")
                       ?? throw new InvalidOperationException("Notifications database connection is not configured.");

builder.Services.AddInfrastructure(connectionString);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(RabbitMqSettings.SectionName));

builder.Services.AddHostedService<RabbitMqConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();