using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var conenctionFactory = new ConnectionFactory
{
    HostName = "localhost"
};

var connection = conenctionFactory.CreateConnection();
using var channel = connection.CreateModel();
channel.QueueDeclare("orders", exclusive: false);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (_, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var data = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Message Received: {data}");
};

channel.BasicConsume("orders", autoAck: true, consumer: consumer);
Console.ReadKey();