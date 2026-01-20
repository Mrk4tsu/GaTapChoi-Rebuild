using Microsoft.AspNetCore.Http;

namespace GaVL.DTO.Profiles
{
    public class AvatarRequest
    {
        public IFormFile AvatarImage { get; set; }
    }
}
