using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Converter.ApiServer.Services
{
    /// <summary>
    /// RabbitMq implementation of IMessageQueueService
    /// </summary>
    public class RabbitMqService : IMessageQueueService
    {
        protected readonly RabbitMqSettings _rabbitMqSettings;
        protected readonly ConnectionFactory _factory;

        public RabbitMqService(RabbitMqSettings rabbitMqSettings)
        {
            _factory = new ConnectionFactory() { HostName = rabbitMqSettings.HostName };
            _rabbitMqSettings = rabbitMqSettings;
        }

        public void SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            SendMessage(message);
        }

        public void SendMessage(string message)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(_rabbitMqSettings.FileConversionQueueName, _rabbitMqSettings.Durable, _rabbitMqSettings.Exclusive, _rabbitMqSettings.AutoDelete);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: _rabbitMqSettings.FileConversionQueueName, basicProperties: null, body: body);
        }
    }
}
