namespace GaVL.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public int RoleId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public Role Role { get; set; }
        public List<Mod> Mods { get; set; }
    }
}
