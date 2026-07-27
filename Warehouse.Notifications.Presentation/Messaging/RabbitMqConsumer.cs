using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Warehouse.Notifications.Application.Notifications.Commands.CreateFileUploadedNotification;
using Warehouse.Notifications.Application.Notifications.Commands.CreateLowStockNotification;
using Warehouse.Notifications.Presentation.IntegrationEvents;

namespace Warehouse.Notifications.Presentation.Messaging;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<RabbitMqConsumer> _logger;

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConsumer(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqSettings> options,
        ILogger<RabbitMqConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _settings = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await StartConsumerAsync(stoppingToken);

                    _logger.LogInformation(
                        "Listening for warehouse events on queue {QueueName}.",
                        _settings.QueueName);

                    await Task.Delay(
                        Timeout.InfiniteTimeSpan,
                        stoppingToken);
                }
                catch (Exception exception)
                    when (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning(
                        exception,
                        "RabbitMQ is unavailable. Retrying in 5 seconds.");

                    await DisposeRabbitMqAsync();

                    await Task.Delay(
                        TimeSpan.FromSeconds(5),
                        stoppingToken);
                }
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            //service shutdown
        }
    }

    private async Task StartConsumerAsync(CancellationToken cancellationToken)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
            AutomaticRecoveryEnabled = true
        };

        var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        _connection = connection;

        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        _channel = channel;

        await channel.ExchangeDeclareAsync(
            exchange: _settings.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: _settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: _settings.QueueName,
            exchange: _settings.ExchangeName,
            routingKey: WarehouseEventRoutingKeys.StockLow,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: _settings.QueueName,
            exchange: _settings.ExchangeName,
            routingKey: WarehouseEventRoutingKeys.FileUploaded,
            cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += (_, eventArgs) => HandleMessageAsync(channel, eventArgs);

        await channel.BasicConsumeAsync(
            queue: _settings.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);
    }

    private async Task HandleMessageAsync(IChannel channel, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var messageBody = eventArgs.Body.ToArray();

            using var scope = _scopeFactory.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

            var wasCreated = eventArgs.RoutingKey switch
            {
                WarehouseEventRoutingKeys.StockLow => await HandleStockLowAsync(mediator, messageBody, eventArgs.CancellationToken),

                WarehouseEventRoutingKeys.FileUploaded => await HandleFileUploadedAsync(mediator, messageBody, eventArgs.CancellationToken),

                _ => throw new InvalidOperationException($"Unsupported routing key: {eventArgs.RoutingKey}")
            };

            await channel.BasicAckAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false);

            _logger.LogInformation("Processed event {RoutingKey}. Notification created: {WasCreated}.", eventArgs.RoutingKey, wasCreated);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to process RabbitMQ event {RoutingKey}.", eventArgs.RoutingKey);

            if (channel.IsOpen)
            {
                await channel.BasicNackAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: false);
            }
        }
    }

    private static Task<bool> HandleStockLowAsync(ISender mediator, byte[] messageBody, CancellationToken cancellationToken)
    {
        var warehouseEvent = JsonSerializer.Deserialize<StockLowDetected>(messageBody)
                             ?? throw new JsonException("The stock-low event body is invalid.");

        var request = new CreateLowStockNotificationRequest(
            warehouseEvent.EventId,
            warehouseEvent.EventTime,
            warehouseEvent.CorrelationId,
            warehouseEvent.RelatedEntityId,
            warehouseEvent.ProductName,
            warehouseEvent.CurrentQuantity,
            warehouseEvent.Threshold);

        return mediator.Send(request, cancellationToken);
    }

    private static Task<bool> HandleFileUploadedAsync(ISender mediator, byte[] messageBody, CancellationToken cancellationToken)
    {
        var warehouseEvent = JsonSerializer.Deserialize<WarehouseFileUploaded>(messageBody)
                             ?? throw new JsonException("The file-upload event body is invalid.");

        var request = new CreateFileUploadedNotificationRequest(
            warehouseEvent.EventId,
            warehouseEvent.EventTime,
            warehouseEvent.CorrelationId,
            warehouseEvent.RelatedEntityId,
            warehouseEvent.SupplierId,
            warehouseEvent.FileName);

        return mediator.Send(request, cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        await DisposeRabbitMqAsync();
    }

    private async Task DisposeRabbitMqAsync()
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
            _channel = null;
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }
}