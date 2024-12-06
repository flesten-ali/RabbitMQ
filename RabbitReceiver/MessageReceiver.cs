using DBRepository;
using Microsoft.Extensions.Configuration;
using Rabbit;
using Rabbit.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
namespace RabbitReceiver;

public class MessageReceiver : IMessageReceiver
{
    private readonly IStatisticsRepository _repo;
    private readonly ConnectionFactory _connectionFactory;
    private readonly SignalRClient _signalRClient;
    private readonly DetectHighUsage _detectHighUsage;
    private readonly DetectAnomaly _detectAnomaly;

    public MessageReceiver(IStatisticsRepository statisticsRepository, IConfiguration configuration, SignalRClient signalRClient)
    {
        _repo = statisticsRepository;
        _signalRClient = signalRClient;

        _connectionFactory = new()
        {
            Uri = new Uri(configuration["RabbitMQConfig:RabbitUrl"] ?? string.Empty),
            ClientProvidedName = "Rabbit Receiver App"
        };

        _detectHighUsage = new(configuration, signalRClient);
        _detectAnomaly = new(configuration, signalRClient);
    }

    public async Task ReceiveAsync()
    {
        var con = await _connectionFactory.CreateConnectionAsync();
        var channel = await con.CreateChannelAsync();

        const string exchangeName = "Exchange";
        const string routingKey = $"ServerStatistics.*";
        const string queueName = "Queue";

        await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct);
        await channel.QueueDeclareAsync(queueName, false, false, false, null);
        await channel.QueueBindAsync(queueName, exchangeName, routingKey, null);
        await channel.BasicQosAsync(0, 1, false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var statisticsObject = JsonSerializer.Deserialize<ServerStatistics>(message);

                if (statisticsObject != null)
                {
                    var receiverServerStatistics = new ReciverServerStatistics
                    {
                        AvailableMemory = statisticsObject.AvailableMemory,
                        ServerIdentifier = args.RoutingKey.Split(".")[1],
                        CpuUsage = statisticsObject.CpuUsage,
                        MemoryUsage = statisticsObject.MemoryUsage,
                        Timestamp = statisticsObject.Timestamp,
                    };

                    await _repo.Add(receiverServerStatistics);
                    await DetectAsync(receiverServerStatistics);
                }

                await channel.BasicAckAsync(args.DeliveryTag, false);
            }
            catch
            {
                throw;
            }
        };

        var consumerTag = await channel.BasicConsumeAsync(queueName, false, consumer);
        Console.ReadLine();
        await channel.BasicCancelAsync(consumerTag);
        await channel.CloseAsync();
        await con.CloseAsync();
    }

    private async Task DetectAsync(ReciverServerStatistics receiverServerStatistics)
    {
        await _signalRClient.StartAsync();

        await _detectHighUsage.CpuHighUsage(receiverServerStatistics);
        await _detectHighUsage.MemoryHighUsage(receiverServerStatistics);
        await _detectAnomaly.CpuUsage(receiverServerStatistics);
        await _detectAnomaly.MemoryUsage(receiverServerStatistics);

        await _signalRClient.StopAsync();
    }
}