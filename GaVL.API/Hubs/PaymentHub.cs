using Microsoft.AspNetCore.SignalR;

namespace GaVL.API.Hubs
{
    public class PaymentHub : Hub
    {
        private readonly ILogger<PaymentHub> _logger;
        public PaymentHub(ILogger<PaymentHub> logger)
        {
            _logger = logger;
        }

        public async Task JoinOrderGroup(string orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
            _logger.LogInformation("Client {ConnectionId} joined group {OrderId}", Context.ConnectionId, orderId);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}
