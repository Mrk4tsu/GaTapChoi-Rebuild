using Microsoft.AspNetCore.Http;

namespace GaVL.DTO.Notification
{
    public class NotifyRequest
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string? Url { get; set; }
        // Optional for update; required for create
        public IFormFile? Thumbnail { get; set; }
    }
}
