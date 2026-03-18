using GaVL.Data;
using GaVL.Data.Entities;
using GaVL.Data.EntityTypes;
using GaVL.DTO.APIResponse;
using GaVL.DTO.Payments;
using Microsoft.EntityFrameworkCore;

namespace GaVL.Application.Payments
{
    public interface IPaymentService
    {
        Task<ApiResult<bool>> HandleSepayWebhook(SepayWebhookData data);
        Task<ApiResult<PaymentType>> CheckPaymentStatus(CheckStatusRequest request);
        Task<ApiResult<Order>> GetOrder(string id);
        Task<ApiResult<string>> CreateOrder(CreateOrderDto request);
    }
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _db;
        private readonly ISignalRPaymentNotifier _notifier;
        public PaymentService(AppDbContext db, ISignalRPaymentNotifier notifier)
        {
            _db = db;
            _notifier = notifier;
        }
        public async Task<ApiResult<PaymentType>> CheckPaymentStatus(CheckStatusRequest request)
        {
            var order = await _db.Orders.FindAsync(request.OrderId);
            if (order == null) return new ApiErrorResult<PaymentType>("Đéo thấy");
            return new ApiSuccessResult<PaymentType>(order.PaymentStatus);
        }
        public async Task<ApiResult<string>> CreateOrder(CreateOrderDto request)
        {
            if (request.Total <= 0) return new ApiErrorResult<string>("Đéo thấy");
            var name = request.Name ?? "Unknow";
            var order = new Order
            {
                Id = Generate8CharTimestamp(),
                Name = name,
                Email = request.Email,
                Total = request.Total,
                NumberPhone = request.NumberPhone,
            };
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<string>(order.Id);
        }
        public async Task<ApiResult<Order>> GetOrder(string id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return new ApiErrorResult<Order>("Đéo thấy");
            return new ApiSuccessResult<Order>(order);
        }
        public async Task<ApiResult<bool>> HandleSepayWebhook(SepayWebhookData data)
        {
            /*
             PSEUDOCODE / PLAN:
             1. Validate input 'data' is not null.
             2. Compute amountIn and amountOut based on TransferType.
             3. Create and persist a PaymentTransaction record.
             4. Trim 'data.Content' and ensure it's not empty.
             5. Split the content into tokens using whitespace and remove empty entries.
             6. Take the last token (safely) as the order id (oId).
             7. Query the Orders table for an order matching:
                - Id == oId
                - Total == amountIn
                - PaymentStatus == UnPaid
             8. If no matching order, return an error result.
             9. Mark order as Paid, save changes, notify via SignalR, and return success.
            */

            if (data == null) return new ApiErrorResult<bool>("Đéo thấy");

            decimal amountIn = data.TransferType == "in" ? data.TransferAmount : 0;
            decimal amountOut = data.TransferType == "out" ? data.TransferAmount : 0;

            var transaction = new PaymentTransaction
            {
                SepayId = data.Id,
                Gateway = data.Gateway,
                TransactionDate = data.TransactionDate,
                AccountNumber = data.AccountNumber,
                SubAccount = data.SubAccount,
                AmountIn = amountIn,
                AmountOut = amountOut,
                Accumulated = data.Accumulated,
                Code = data.Code,
                TransactionContent = data.Content,
                ReferenceNumber = data.ReferenceCode,
                Body = data.Description
            };

            _db.Payments.Add(transaction);
            await _db.SaveChangesAsync();

            var content = data.Content?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(content))
            {
                return new ApiErrorResult<bool>("Order not found. Content is empty");
            }

            // Split on any whitespace and remove empty entries, then take the last token as order id
            var parts = content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return new ApiErrorResult<bool>("Order not found. Content parsing failed");
            }
            var oId = parts[^1];

            var order = await _db.Orders
                .FirstOrDefaultAsync(o => o.Id == oId && o.Total == amountIn && o.PaymentStatus == PaymentType.UnPaid);
            if (order == null)
            {
                return new ApiErrorResult<bool>($"Order not found. Order_id {oId}");
            }
            order.PaymentStatus = PaymentType.Paid;
            await _db.SaveChangesAsync();
            await _notifier.NotifyPaymentSuccessAsync(order.Id);
            return new ApiSuccessResult<bool>();
        }
        private string Generate8CharTimestamp()
        {
            long seconds = DateTimeOffset.UtcNow.Ticks;
            const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Span<char> buffer = stackalloc char[8];

            ulong value = (ulong)seconds;
            for (int i = 7; i >= 0; i--)
            {
                buffer[i] = alphabet[(int)(value % 36)];
                value /= 36;
            }
            return new string(buffer);
        }
    }
}
