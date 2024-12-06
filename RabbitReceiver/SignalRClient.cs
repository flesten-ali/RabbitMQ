using Microsoft.AspNetCore.SignalR.Client;
namespace RabbitReceiver;

public class SignalRClient
{
    private readonly HubConnection _con;

    public SignalRClient(string url)
    {
        _con = new HubConnectionBuilder().WithUrl(url).Build();
        _con.On<string>("ReceiveAlert", Console.WriteLine);
    }

    public async Task StartAsync()
    {
        if (_con.State == HubConnectionState.Disconnected)
        {
            try
            {
                await _con.StartAsync();
                Console.WriteLine("Connection to SignalR established Successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public async Task StopAsync()
    {
        if (_con.State == HubConnectionState.Connected)
        {
            try
            {
                await _con.StopAsync();
                Console.WriteLine("Disconnected to SignalR Successfully!");
            }
            catch
            {
                throw;
            }
        }
    }

    public async Task SendAlertAsync(string message)
    {
        if (_con.State == HubConnectionState.Connected)
        {
            try
            {
                await _con.InvokeAsync("SendAlert", message);
            }
            catch
            {
                throw;
            }
        }
        else
        {
            Console.WriteLine("Not Connected!");
        }
    }
}