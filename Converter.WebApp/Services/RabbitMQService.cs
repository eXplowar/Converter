using Converter.WebApp.Hubs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;

namespace Converter.WebApp.Services
{
    public class RabbitMQService : IMessageQueueListenService
    {
        protected readonly ILogger<RabbitMQService> _logger;
        protected readonly ConnectionFactory _factory;
        protected readonly RabbitMqSettings _rabbitMqSettings;
        protected readonly RepositoryHub _repositoryHub;
        protected readonly TimeSpan RECONNECTION_TIMEOUT;
        protected readonly object _lock = new();
        protected IConnection _connection;
        protected IModel _channel;
        protected bool _connectionEventsHandled;

        public RabbitMQService(RepositoryHub repositoryHub, RabbitMqSettings rabbitMqSettings, ILogger<RabbitMQService> logger)
        {
            _logger = logger;
            _rabbitMqSettings = rabbitMqSettings;
            _repositoryHub = repositoryHub;

            _factory = new ConnectionFactory() { HostName = rabbitMqSettings.HostName };

            RECONNECTION_TIMEOUT = _factory.NetworkRecoveryInterval; // defaults to 5s
        }

        public virtual void ConnectAndSubscribe()
        {
            InitConnection();

            _channel.QueueDeclare(_rabbitMqSettings.FileConversionQueueName, _rabbitMqSettings.Durable, _rabbitMqSettings.Exclusive, _rabbitMqSettings.AutoDelete);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, basicDeliverEventArgs) =>
            {
                var response = Encoding.UTF8.GetString(basicDeliverEventArgs.Body.ToArray());
                var message = JsonSerializer.Deserialize<StorageKey>(response);

                await _repositoryHub.SendMessage(_repositoryHub.Context, message.FileHash, message.Token);

                if (!_rabbitMqSettings.AutoAck)
                {
                    _channel.BasicAck(basicDeliverEventArgs.DeliveryTag, false);
                }
            };

            _channel.BasicConsume(_rabbitMqSettings.FileConversionQueueName, _rabbitMqSettings.AutoAck, consumer);
        }

        /// <summary>
        /// Initializing RabbitMQ connection.
        /// </summary>
        private void InitConnection()
        {
            lock (_lock)
            {
                while (!(_connection != null && _connection.IsOpen))
                {
                    try
                    {
                        _connection = _factory.CreateConnection();
                        _logger.LogInformation($"RabbitMQ connected");
                        break;
                    }
                    catch (BrokerUnreachableException e)
                    {
                        _logger.LogError($"No connection to RabbitMQ. Rabbit host: {_factory.HostName}. Reconnection after {RECONNECTION_TIMEOUT.TotalSeconds}s");
                        Thread.Sleep(RECONNECTION_TIMEOUT);
                    }
                }

                HandleConnectionEvents();

                _channel = _connection.CreateModel();
            }
        }

        private void HandleConnectionEvents()
        {
            if (_connectionEventsHandled) return;

            _connection.ConnectionShutdown += (object sender, ShutdownEventArgs e) =>
            {
                _logger.LogError($"Connection to RabbitMQ is lost.");
                InitConnection();
            };

            _connectionEventsHandled = true;
        }
    }

    internal record StorageKey(string FileName, int FileHash, string Token);
}
