using GaVL.Application.Payments;
using GaVL.Application.Systems;
using GaVL.DTO.Payments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GaVL.API.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [HttpPost("check-status")]
        public async Task<IActionResult> CheckPaymentStatus([FromBody] CheckStatusRequest request)
        {
            var result = await _paymentService.CheckPaymentStatus(request);
            return Ok(result);
        }
        [HttpPost("order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto request)
        {
            var result = await _paymentService.CreateOrder(request);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(string id)
        {
            var order = await _paymentService.GetOrder(id);
            return Ok(order);
        }
    }
}
