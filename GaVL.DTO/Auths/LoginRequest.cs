namespace GaVL.DTO.Auths
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string CaptchaToken { get; set; }
    }
    public class LoginDashboardRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
