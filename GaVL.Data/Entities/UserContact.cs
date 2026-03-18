namespace GaVL.Data.Entities
{
    public class UserContact
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Platform/Provider name. e.g., "Facebook", "Zalo", "Website", "Github"
        /// </summary>
        public string Provider { get; set; }
        
        /// <summary>
        /// The actual URL, phone number, or username
        /// </summary>
        public string Value { get; set; }
        
        /// <summary>
        /// Optional custom label (e.g., "My Developer Blog")
        /// </summary>
        public string? DisplayLabel { get; set; }
        
        public bool IsPublic { get; set; } = true;
        
        public int Position { get; set; } = 0;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public User User { get; set; }
    }
}
