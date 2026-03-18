using GaVL.API.Hubs;
using GaVL.Application.Payments;
using Microsoft.AspNetCore.SignalR;

namespace GaVL.API.Extensions
{
    public class SignalRPaymentNotifier : ISignalRPaymentNotifier
    {
        private readonly IHubContext<PaymentHub> _hubContext;

        public SignalRPaymentNotifier(IHubContext<PaymentHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyPaymentSuccessAsync(string orderId)
        {
            await _hubContext.Clients.Group(orderId).SendAsync("PaymentSuccess", orderId);
        }
    }
}
