using System.ComponentModel.DataAnnotations;

namespace GaVL.DTO.Profiles
{
    public class ContactRequest
    {
        [Required]
        [MaxLength(50)]
        public string Provider { get; set; }

        [Required]
        [MaxLength(500)]
        public string Value { get; set; }

        [MaxLength(100)]
        public string? DisplayLabel { get; set; }

        public bool IsPublic { get; set; } = true;

        public int Position { get; set; } = 0;
    }
}
