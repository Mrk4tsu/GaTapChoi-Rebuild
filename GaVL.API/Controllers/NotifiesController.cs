using GaVL.API.Hubs;
using GaVL.Application.Systems;
using GaVL.DTO.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GaVL.API.Controllers
{
    [Route("api/notify")]
    [ApiController]
    public class NotifiesController : BasesController
    {
        private readonly INotifyService _notifyService;
        private readonly IHubContext<NotifyHub> _hubContext;
        private readonly ILogger<NotifiesController> _logger;
        public NotifiesController(ILogger<NotifiesController> logger, IHubContext<NotifyHub> hubContext, INotifyService notifyService)
        {
            _logger = logger;
            _hubContext = hubContext;
            _notifyService = notifyService;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _notifyService.GetListNotify();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _notifyService.GetById(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] NotifyRequest request, CancellationToken ct)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _notifyService.Create(request, userId.Value, ct);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromForm] NotifyRequest request, CancellationToken ct)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _notifyService.Update(id, request, userId.Value, ct);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _notifyService.Delete(id, userId.Value, ct);
            return Ok(result);
        }
        [HttpPost("hook")]
        public async Task<IActionResult> MemberJoined([FromBody] MemberJoinedDto dto, string key)
        {
            if (key != "abc@123")
            {
                return Unauthorized("Invalid API Key");
            }
            _logger.LogInformation(
                "Member joined: {Username} ({UserId}) in {GuildName}",
                dto.Username, dto.UserId, dto.GuildName
            );

            await _hubContext.Clients.All.SendAsync("MemberJoined", dto);
            if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.AvatarUrl))
            {
                return BadRequest("Thiếu dữ liệu, kiểm tra Avatar hoặc Username");
            }
            return Ok("Đã nhận dữ liệu");
        }
        [HttpPost("news"), Authorize]
        public async Task<IActionResult> Notify()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized("User ID not found in claims");
            }
            return Ok("API hoạt động bình thường");
        }
        [HttpPost("remove-cache")]
        public async Task<IActionResult> RemoveCache()
        {
            await _notifyService.InvalidateCache();
            return Ok();
        }
    }
}
