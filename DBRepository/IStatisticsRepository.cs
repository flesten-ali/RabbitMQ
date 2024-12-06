using Rabbit.Models;
namespace DBRepository;

public interface IStatisticsRepository
{
    Task Add(ReciverServerStatistics receiverServerStatistics);
}
