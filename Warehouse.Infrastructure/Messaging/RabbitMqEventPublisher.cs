using System.Text.Json;
using RabbitMQ.Client;
using Warehouse.Application.IntegrationEvents;
using Warehouse.Application.Interfaces;

namespace Warehouse.Infrastructure.Messaging;

public class RabbitMqEventPublisher : IWarehouseEventPublisher, IAsyncDisposable
{
    private readonly RabbitMqSettings _rabbitMqSettings;
    private readonly ConnectionFactory _connectionFactory;
    private readonly SemaphoreSlim _channelLock =  new SemaphoreSlim(1, 1);
    
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqEventPublisher(RabbitMqSettings rabbitMqSettings)
    {
        _rabbitMqSettings = rabbitMqSettings;
        _connectionFactory = new ConnectionFactory
        {
            HostName = _rabbitMqSettings.HostName,
            Port = _rabbitMqSettings.Port,
            UserName = _rabbitMqSettings.UserName,
            Password = _rabbitMqSettings.Password,
            AutomaticRecoveryEnabled = true,
        };
    }

    public async Task PublishAsync<TEvent>(TEvent warehouseEvent, string routingKey, CancellationToken cancellationToken)
        where TEvent : WarehouseEvent
    {
        await _channelLock.WaitAsync(cancellationToken);

        try
        {
            var channel = await GetChannelAsync(cancellationToken);
            var messageBody = JsonSerializer.SerializeToUtf8Bytes(warehouseEvent);
            var properties = new BasicProperties
            {
                ContentType = "application/json",
                Persistent = true,
            };

            await channel.BasicPublishAsync(exchange: _rabbitMqSettings.ExchangeName, routingKey: routingKey,
                mandatory: false, basicProperties: properties, body: messageBody, cancellationToken: cancellationToken);
        }
        finally
        {
            _channelLock.Release();
        }
    }
    

    private async Task<IChannel> GetChannelAsync(
        CancellationToken cancellationToken)
    {
        var connection = _connection;

        if (connection is null || !connection.IsOpen)
        {
            connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

            _connection = connection;
            _channel = null;
        }

        var channel = _channel;

        if (channel is null || !channel.IsOpen)
        {
            channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.ExchangeDeclareAsync(exchange: _rabbitMqSettings.ExchangeName, type: ExchangeType.Topic, durable: true,
                autoDelete: false, arguments: null, cancellationToken: cancellationToken);

            _channel = channel;
        }

        return channel;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync();

        if (_connection is not null)
            await _connection.DisposeAsync();

        _channelLock.Dispose();
    }
}
