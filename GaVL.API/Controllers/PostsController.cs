using GaVL.Application.Catalog.Posts;
using GaVL.DTO.Paging;
using GaVL.DTO.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GaVL.API.Controllers
{
    [Route("api/post")]
    [ApiController]
    public class PostsController : BasesController
    {
        private readonly IPostService _postService;
        public PostsController(IPostService postService)
        {
            _postService = postService;
        }
        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] PagingRequest request)
        {
            var result = await _postService.GetPosts(request);
            return Ok(result);
        }
        [HttpPost, Authorize]
        public async Task<IActionResult> CreatePost(PostRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _postService.CreatePost(request, userId.Value);
            return Ok(result);
        }

    }
}
