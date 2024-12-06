using Rabbit.BusinessLogic;
using Rabbit.Models;
using RabbitSender;
using System.Diagnostics;

public class Program
{
    public static async Task Main(string[] args)
    {
        var memoryUsage = new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName);
        var availableMemory = new PerformanceCounter("Memory", "Available MBytes");
        var cpuUsage = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        var configurationObject = ServerConfigration.GetConfigrationObject();
        MessageSender ms = new("amqp://guest:guest@localhost:5672");

        while (true)
        {
            var memoryUsageInMb = (memoryUsage.NextValue()) / (1024 * 1024);
            var availableMemoryInMb = (availableMemory.NextValue());

            cpuUsage.NextValue();
            Thread.Sleep(1000);
            var cpuUsagePercentage = (cpuUsage.NextValue());

            var serverStatistics = new ServerStatistics
            {
                AvailableMemory = memoryUsageInMb,
                CpuUsage = cpuUsagePercentage,
                MemoryUsage = memoryUsageInMb,
                Timestamp = DateTime.Now,
            };
            await ms.SendAsync(serverStatistics, configurationObject.ServerIdentifier);
            Thread.Sleep(configurationObject.SamplingIntervalSeconds);
        }
    }
}