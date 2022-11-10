namespace Converter.ApiServer.Models
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; }
        public string FileConversionQueueName { get; set; }
        public bool Durable { get; set; } = false;
        public bool Exclusive { get; set; } = false;
        public bool AutoDelete { get; set; } = false;
        public bool AutoAck { get; set; } = true;
    }
}
