using Microsoft.AspNetCore.SignalR;
namespace SignalR;

public class AlertHub:Hub
{
    public async Task SendAlert(string message) // server 
    {
        await Clients.All.SendAsync("ReceiveAlert", message); // send message to all clients
    }
}
