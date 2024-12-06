using Microsoft.Extensions.Configuration;
using Rabbit.Models;
namespace RabbitReceiver;

public class DetectHighUsage
{
    private readonly IConfiguration _configuration;
    private readonly SignalRClient _signalRClient;

    public DetectHighUsage(IConfiguration configuration, SignalRClient client)
    {
        _configuration = configuration;
        _signalRClient = client;
    }

    public async Task CpuHighUsage(ReciverServerStatistics cur)
    {
        var cpuThreshold = double.Parse(_configuration["AnomalyDetectionConfig:CpuUsageThresholdPercentage"] ?? string.Empty);
        if (cur.CpuUsage > cpuThreshold)
        {
            await _signalRClient.SendAlertAsync("Cpu High Usage!");
        }
    }

    public async Task MemoryHighUsage(ReciverServerStatistics cur)
    {
        var totalMemory = cur.MemoryUsage + cur.AvailableMemory;
        var memoryThreshold = double.Parse(_configuration["AnomalyDetectionConfig:MemoryUsageThresholdPercentage"] ?? string.Empty);
        if ((cur.MemoryUsage / totalMemory) > memoryThreshold)
        {
            await _signalRClient.SendAlertAsync("Memory High Usage!");
        }
    }
}