using Rabbit.Models;
using System.Text.Json;
namespace Rabbit.BusinessLogic;

public class ServerConfigration
{
    public static ServerStatisticsConfig GetConfigrationObject()
    {
        try
        {
            var config = JsonSerializer.Deserialize<StatisticsConfig>(File.ReadAllText("appsettings.json"));
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
