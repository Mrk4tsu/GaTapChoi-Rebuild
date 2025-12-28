namespace GaVL.DTO.Mods
{
    public class ModUpdateCombineRequest : ModCombineRequest
    {
        public List<UrlUpdateRequest>? UpdatedUrls { get; set; }
        public List<int>? UrlIdsToDelete { get; set; }
    }
    public class UpdateModRequest
    {
        public List<UrlUpdateRequest>? UpdatedUrls { get; set; }
        public List<int>? UrlIdsToDelete { get; set; }
    }
    public class UrlUpdateRequest
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
    }
    public class ModInner
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPrivate { get; set; } = false;
        public byte CategoryId { get; set; }
        public Guid AuthorId { get; set; }
        public List<UrlModDTO> Urls { get; set; } = new List<UrlModDTO>();
    }
}
