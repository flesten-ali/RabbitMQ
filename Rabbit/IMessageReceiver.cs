namespace Rabbit;

public interface IMessageReceiver
{
    Task ReceiveAsync();
}
