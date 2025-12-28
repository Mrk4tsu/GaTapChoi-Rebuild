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
    }
}
