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
}
