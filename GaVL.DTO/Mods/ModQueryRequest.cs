using GaVL.DTO.Paging;

namespace GaVL.DTO.Mods
{
    public class ModQueryRequest : PagingRequest
    {
        public string? Username { get; set; }
        public byte? CategoryId { get; set; }
    }
}
