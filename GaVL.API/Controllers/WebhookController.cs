using GaVL.Application.Payments;
using GaVL.DTO.Payments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GaVL.API.Controllers
{
    [Route("api/webhook")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public WebhookController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [HttpPost("sepay")]
        public async Task<IActionResult> HandleSepayWebhook([FromBody] SepayWebhookData data)
        {
            var result = await _paymentService.HandleSepayWebhook(data);
            return Ok(result);
        }
    }
}
