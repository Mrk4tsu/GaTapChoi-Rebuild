namespace GaVL.DTO.Mods
{
    public class ModCombineRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPrivate { get; set; } = false;
        public byte CategoryId { get; set; }

        public List<string>? NewUrls { get; set; }
    }
    public class ModRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPrivate { get; set; } = false;
        public byte CategoryId { get; set; }
    }
    public class UrlRequest
    {
        public List<string>? Url { get; set; }
    }
}
