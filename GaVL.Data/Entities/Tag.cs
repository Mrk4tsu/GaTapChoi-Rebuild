namespace GaVL.Data.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SeoAlias { get; set; }
        public List<PostTag> PostTags { get; set; }
    }
}
