using System.ComponentModel.DataAnnotations;

namespace GaVL.DTO.Profiles
{
    public class UpdateUserInfoRequest
    {
        [EmailAddress]
        public string? Email { get; set; }

        public List<ContactRequest>? Contacts { get; set; }
    }
}
