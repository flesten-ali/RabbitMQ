using DBRepository;
using Microsoft.Extensions.Configuration;
namespace RabbitReceiver;

class Program
{
    public static async Task Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();

        var url = configuration["SignalRConfig:SignalRUrl"];
        if (url != null)
        {
            SignalRClient signalRClient = new(url);
            MessageReceiver receiver = new(new MongoRepository(), configuration, signalRClient);
            await receiver.ReceiveAsync();
        }

        Console.ReadLine();
    }
}