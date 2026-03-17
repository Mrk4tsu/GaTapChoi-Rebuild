using GaVL.Application.Catalog.Common;
using Microsoft.AspNetCore.Mvc;

namespace GaVL.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly SearchService _searchService;

        public SearchController(SearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("suggest")]
        public async Task<IActionResult> Suggest([FromQuery] string q)
        {
            var results = await _searchService.GetSuggestionsAsync(q);
            return Ok(results);
        }
    }
}