namespace GaVL.Data.Entities
{
    public class PostRevision
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public Guid UserId { get; set; }
        public string ContentSnapshot { get; set; }
        public DateTime CreatedAt { get; set; }
        public User User { get; set; }
    }
}
