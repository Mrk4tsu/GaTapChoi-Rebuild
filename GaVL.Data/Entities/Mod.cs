using System;

namespace GaVL.Data.Entities
{
    public class Mod
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public bool IsLocked { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UserId { get; set; }
        public int ViewCount { get; set; }
        public User User { get; set; } = null!;
        public byte CategoryId { get; set; }
        public byte CrackType { get; set; }
        public string SeoAlias { get; set; }
        public Category Category { get; set; }
        public bool IsPrivate { get; set; }
    }
}
