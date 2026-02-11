namespace GaVL.DTO.Auths
{
    public class GoogleLoginDto
    {
        public string AccessToken { get; set; }
    }
    public class GoogleUserInfoResponse
    {
        public string Sub { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
    }
}
