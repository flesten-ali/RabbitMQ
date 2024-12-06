using Rabbit.Models;
namespace Rabbit;

public interface IMessageSender
{
    Task SendAsync(ServerStatistics serverStatistics, string serverIdentifier);
}
