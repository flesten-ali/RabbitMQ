using Rabbit;
using Rabbit.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
namespace RabbitSender;

public class MessageSender : IMessageSender
{
    private readonly ConnectionFactory _connectionFactory;

    public MessageSender(string connectionString)
    {
        _connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };
    }

    public async Task SendAsync(ServerStatistics serverStatistics, string serverIdentifier)
    {
        _connectionFactory.ClientProvidedName = "Rabbit Publisher App";

        var con = await _connectionFactory.CreateConnectionAsync();
        var channel = await con.CreateChannelAsync();

        const string exchangeName = "Exchange";
        var routingKey = $"ServerStatistics.{serverIdentifier}";
        const string queueName = "Queue";

        await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct);
        await channel.QueueDeclareAsync(queueName, false, false, false, null);
        await channel.QueueBindAsync(queueName, exchangeName, routingKey, null);

        var jsonObject = JsonSerializer.Serialize(serverStatistics);
        var msgBytes = Encoding.UTF8.GetBytes(jsonObject);
        await channel.BasicPublishAsync(exchangeName, routingKey, msgBytes);

        await channel.CloseAsync();
        await con.CloseAsync();
    }
}
