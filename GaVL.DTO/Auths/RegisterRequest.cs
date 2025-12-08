namespace GaVL.DTO.Auths
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? AvatarUrl { get; set; }
        public string TurnstileToken { get; set; }
    }
}
