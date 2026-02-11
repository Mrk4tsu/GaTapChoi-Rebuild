namespace GaVL.Data.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public int? RootId { get; set; }
        public int? ParentId { get; set; }
        public Comment? CommentRoot { get; set; }
        public Comment? CommentParent { get; set; }
        public List<Comment> Children { get; set; }
    }
}
