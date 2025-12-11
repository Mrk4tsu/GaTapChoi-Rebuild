namespace GaVL.DTO.Mods
{
    public class ModDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Username { get; set; }
        public string CategoryName { get; set; }
        public byte CrackType { get; set; }
        public string SeoAlias { get; set; }
        public bool IsPrivate { get; set; }
    }
    public class ModDetailDTO : ModDTO
    {
        public string Description { get; set; }
        public int ViewCount { get; set; }
        public string CategorySeoAlias { get; set; }
    }
}
