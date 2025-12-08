using GaVL.DTO.Auths;

namespace GaVL.Application.Auths
{
    public interface IRegistrationService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request, string ipAddress);
        Task<bool> ValidateTurnstileTokenAsync(string token, string ipAddress);
    }
    public class RegistrationService : IRegistrationService
    {
        public Task<RegisterResponse> RegisterAsync(RegisterRequest request, string ipAddress)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateTurnstileTokenAsync(string token, string ipAddress)
        {
            throw new NotImplementedException();
        }
    }
}
