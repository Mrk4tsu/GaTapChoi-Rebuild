using Microsoft.AspNetCore.Http;

namespace GaVL.DTO.Notification
{
    public class NotifyRequest
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string? Url { get; set; }
        public IFormFile Thumbnail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
