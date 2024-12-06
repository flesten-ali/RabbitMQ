using SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

var app = builder.Build();

app.UseHttpsRedirection();
 
app.MapHub<AlertHub>("/alertHub");

app.Run();
