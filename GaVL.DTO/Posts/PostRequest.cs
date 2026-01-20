using Microsoft.AspNetCore.Http;

namespace GaVL.DTO.Posts
{
    public class PostRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile Thumbnail { get; set; }
        public string Sumary { get; set; }
        public int CategoryId { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}
