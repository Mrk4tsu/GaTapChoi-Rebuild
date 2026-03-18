namespace GaVL.Application.Payments
{
    public interface ISignalRPaymentNotifier
    {
        Task NotifyPaymentSuccessAsync(string orderId);
    }
}
