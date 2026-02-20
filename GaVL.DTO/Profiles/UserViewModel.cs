namespace GaVL.DTO.Profiles
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
    }
}
