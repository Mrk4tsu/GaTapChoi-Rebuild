using GaVL.DTO.Auths;
using GaVL.DTO.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace GaVL.Application.Systems
{
    public interface ITurnstileService
    {
        Task<bool> ValidateTokenAsync(string token, string remoteIp = null);
    }
    public class TurnstileService : ITurnstileService
    {
        private readonly TurnstileSetting _settings;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TurnstileService(IOptions<TurnstileSetting> settings, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _settings = settings.Value;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> ValidateTokenAsync(string token, string remoteIp = null)
        {
            // Nếu chạy môi trường DEV thì bỏ qua
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.Equals(env, "Development", StringComparison.OrdinalIgnoreCase))
                return true;

            if (string.IsNullOrEmpty(token)) return false;
            remoteIp ??= _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            var request = new Dictionary<string, string>
            {
                { "secret", _settings.SecretKey },
                { "response", token },
                { "remoteip", remoteIp }
            };
            var response = await _httpClient.PostAsync(_settings.VerifyUrl, new FormUrlEncodedContent(request));
            if (!response.IsSuccessStatusCode) return false;
            var result = await response.Content.ReadFromJsonAsync<TurnstileResponse>();
            return result?.Success ?? false;
        }
    }
}
