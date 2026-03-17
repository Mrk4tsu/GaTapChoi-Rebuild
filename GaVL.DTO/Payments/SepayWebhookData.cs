using System.Text.Json.Serialization;

namespace GaVL.DTO.Payments
{
    public class SepayWebhookData
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } // Mới: ID giao dịch SePay

        [JsonPropertyName("gateway")]
        public string? Gateway { get; set; }

        [JsonPropertyName("transactionDate")]
        public string? TransactionDate { get; set; }

        [JsonPropertyName("accountNumber")]
        public string? AccountNumber { get; set; }

        [JsonPropertyName("subAccount")]
        public string? SubAccount { get; set; }

        [JsonPropertyName("transferType")]
        public string? TransferType { get; set; }

        [JsonPropertyName("transferAmount")]
        public decimal TransferAmount { get; set; }

        [JsonPropertyName("accumulated")]
        public decimal Accumulated { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("referenceCode")]
        public string? ReferenceCode { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
    public class SePayWebhookDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;


        [JsonPropertyName("gateway")]
        public string? Gateway { get; set; }


        [JsonPropertyName("transactionDate")]
        public DateTime? TransactionDate { get; set; }


        [JsonPropertyName("accountNumber")]
        public string? AccountNumber { get; set; }


        [JsonPropertyName("transferType")]
        public string? TransferType { get; set; }


        [JsonPropertyName("transferAmount")]
        public decimal? TransferAmount { get; set; }


        [JsonPropertyName("description")]
        public string? Description { get; set; }


        // Optional: signature if SePay provides one in future
        [JsonPropertyName("signature")]
        public string? Signature { get; set; }
    }
}
