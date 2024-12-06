using Rabbit.Models;
using System.Text.Json;
namespace Rabbit.BusinessLogic;

public class ServerConfigration
{
    public static ServerStatisticsConfig GetConfigrationObject()
    {
        try
        {
            var config = JsonSerializer.Deserialize<StatisticsConfig>(File.ReadAllText("C:\\Users\\User\\Desktop\\RabbitTutorial\\RabbitExercise\\Rabbit\\appsettings.json"));
            return new ServerStatisticsConfig
            {
                SamplingIntervalSeconds = config.ServerStatisticsConfig.SamplingIntervalSeconds,
                ServerIdentifier = config.ServerStatisticsConfig.ServerIdentifier,
            };
        }
        catch
        {
            throw;
        }
    }
}
