using Microsoft.AspNetCore.SignalR;

namespace GaVL.API.Hubs
{
    public class NotifyHub : Hub
    {
        private readonly ILogger<NotifyHub> _logger;
        public NotifyHub(ILogger<NotifyHub> logger) 
        {
            _logger = logger;
        }
        public async Task SendMessageToAll(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            _logger.LogInformation("Message sent to all clients: {User} - {Message}", user, message);
        }
        public async Task MemberJoined(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            _logger.LogInformation("Message sent to all clients: {User} - {Message}", user, message);
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}
