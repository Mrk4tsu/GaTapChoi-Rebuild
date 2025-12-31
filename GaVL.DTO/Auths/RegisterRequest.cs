using Microsoft.AspNetCore.Http;

namespace GaVL.DTO.Auths
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public IFormFile? Avatar { get; set; }
        public string CaptchaToken { get; set; }
    }
}
