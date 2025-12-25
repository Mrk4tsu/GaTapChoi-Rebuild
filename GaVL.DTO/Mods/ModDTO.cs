namespace GaVL.DTO.Mods
{
    public class ModDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Thumbnail { get; set; }
        public string Username { get; set; }
        public string CategoryName { get; set; }
        public byte CategoryId { get; set; }
        public byte CrackType { get; set; }
        public string SeoAlias { get; set; }
        public bool IsPrivate { get; set; }
    }
    public class ModDetailDTO : ModDTO
    {
        public Guid AuthorId { get; set; }
        public string Description { get; set; }
        public int ViewCount { get; set; }
        public List<UrlModDTO> Urls { get; set; }
    }
    public class UrlModDTO
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }
    public class SeoModDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SeoAlias { get; set; }
    }
}
