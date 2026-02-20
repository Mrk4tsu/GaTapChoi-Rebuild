using GaVL.API.Hubs;
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
        private readonly IHubContext<NotifyHub> _hubContext;
        private readonly ILogger<NotifiesController> _logger;
        public NotifiesController(ILogger<NotifiesController> logger, IHubContext<NotifyHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
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
    }
}
