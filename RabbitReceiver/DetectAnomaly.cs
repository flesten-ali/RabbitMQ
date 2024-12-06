using Microsoft.Extensions.Configuration;
using Rabbit.Models;
namespace RabbitReceiver;

public class DetectAnomaly
{
    private readonly IConfiguration _configuration;

    private readonly SignalRClient _signalRClient;
    public double PreviousMemoryUsage { get; private set; }
    private double PreviousCpuUsage { get; set; }

    public DetectAnomaly(IConfiguration configuration, SignalRClient client)
    {
        _configuration = configuration;
        _signalRClient = client;
    }

    public async Task CpuUsage(ReciverServerStatistics cur)
    {
        if (PreviousCpuUsage == 0)
        {
            PreviousCpuUsage = cur.CpuUsage;
        }
        else
        {
            var cpuThreshold = double.Parse(_configuration["AnomalyDetectionConfig:CpuUsageAnomalyThresholdPercentage"] ?? string.Empty);

            if (cur.CpuUsage > (PreviousCpuUsage * (1 + cpuThreshold)))
            {
                await _signalRClient.SendAlertAsync("CPU Usage Anomaly!");
            }
        }
    }

    public async Task MemoryUsage(ReciverServerStatistics cur)
    {
        var memoryThreshold = double.Parse(_configuration["AnomalyDetectionConfig:MemoryUsageAnomalyThresholdPercentage"] ?? string.Empty);
        if (cur.MemoryUsage > (PreviousMemoryUsage * (1 + memoryThreshold)))
        {
            await _signalRClient.SendAlertAsync("Memory Usage Anomaly!");
        }
    }
}