using System.Text.Json.Serialization;

namespace GaVL.DTO.Auths
{
    public class TurnstileResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}
