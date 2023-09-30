using Contracts.Common.Interfaces;
using Contracts.Messages;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Shared.Configurations;
using System.Text;

namespace Infrastructure.Messages
{
    public class RabbitMQProducer : IMessageProducer
    {
        private readonly ISerializeService _serializeService;
        private readonly IConfiguration _configuration;

        public RabbitMQProducer(ISerializeService serializeService, IConfiguration configuration)
        {
            _serializeService = serializeService;
            _configuration = configuration;
        }

        public RabbitMQProducer()
        {

        }

        public void SendMessage<T>(T message)
        {
            var hostName = _configuration.GetValue<string>("")
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
            };

            var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare("orders", exclusive: false);
            var jsonData = _serializeService.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonData);
            channel.BasicPublish(exchange: "", routingKey: "orders", body: body);
        }
    }
}
