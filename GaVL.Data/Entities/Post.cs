namespace GaVL.Data.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Code { get; set; }
        public string MainImage {  get; set; }
        public string Title {  get; set; }
        public string Description { get; set; }
        public string SeoAlias { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDeleted { get; set; }
        public User User { get; set; }
    }
}
