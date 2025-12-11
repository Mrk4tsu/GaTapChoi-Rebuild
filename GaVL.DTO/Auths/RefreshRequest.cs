namespace GaVL.DTO.Auths
{
    public class RefreshRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string SessionId { get; set; }
    }
}
