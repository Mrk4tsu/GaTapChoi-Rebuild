namespace GaVL.Data.Entities
{
    public class PaymentTransaction
    {
        public int Id { get; set; } // Auto-generated PK
        public int? SepayId { get; set; } // Mới: Lưu ID từ SePay (nullable nếu không luôn có)
        public string? Gateway { get; set; }
        public string? TransactionDate { get; set; }
        public string? AccountNumber { get; set; }
        public string? SubAccount { get; set; }
        public decimal? AmountIn { get; set; }
        public decimal? AmountOut { get; set; }
        public decimal? Accumulated { get; set; }
        public string? Code { get; set; }
        public string? TransactionContent { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Body { get; set; }
    }
}
