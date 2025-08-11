using Microsoft.AspNetCore.Mvc;
using PaytrAuthApi.Models;
using PaytrAuthApi.Services;

namespace PaytrAuthApi.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartPayment(
            [FromBody] PaymentRequestModel request,
            [FromHeader(Name = "Authorization")] string? authHeader)
        {
            if (string.IsNullOrWhiteSpace(request.UserIp) || request.Amount <= 0 || request.UserBasket == null || !request.UserBasket.Any())
            {
                return BadRequest(new
                {
                    result = "fail",
                    message = "Geçersiz istek. Gerekli alanlar eksik veya hatalı."
                });
            }

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new
                {
                    result = "fail",
                    message = "Authorization header eksik veya hatalı."
                });
            }

            var token = authHeader.Substring("Bearer ".Length);

            // token'ı service'e parametre olarak gönder
            var response = await _paymentService.StartPaymentAsync(request, token);
            return Ok(response);
        }
    }
}