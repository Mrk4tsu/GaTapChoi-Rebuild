namespace GaVL.DTO.Posts
{
    public class PostDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName {  get; set; }
        public Guid AuthorId { get; set; }
        public string SeoAlias {  get; set; }
        public string Thumbnail {  get; set; }
        public string Sumary {  get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class PostDetailDTO : PostDTO
    {
        public string Content { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<string> Tags { get; set; }
    }
    public class SeoPostDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SeoAlias { get; set; }
        public string Thumbnail { get; set; }
    }
}
