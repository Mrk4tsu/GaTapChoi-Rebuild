using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaVL.DTO.Common
{
    public class SearchSuggestionDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Phân biệt "Mod" hoặc "Post"
        public string SeoAlias { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
    }
}
