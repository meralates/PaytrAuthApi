using Microsoft.AspNetCore.Mvc;
using PaytrAuthApi.Models;
using PaytrAuthApi.Services;
using PaytrAuthApi.Settings;
using Microsoft.Extensions.Options;

namespace PaytrAuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToraController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly PaymentService _paymentService;
        private readonly PaytrSettings _paytrSettings;

        public ToraController(
            AuthService authService,
            PaymentService paymentService,
            IOptions<PaytrSettings> paytrSettings)
        {
            _authService = authService;
            _paymentService = paymentService;
            _paytrSettings = paytrSettings.Value;
        }

        [HttpPost("payment")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestModel request)
        {
            
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Unauthorized(new { result = "fail", message = "Authorization header eksik." });
            }

            var authHeader = Request.Headers["Authorization"].ToString();
            if (!_authService.ValidateBasicAuth(authHeader))
            {
                return Unauthorized(new { result = "fail", message = "Geçersiz kullanıcı adı veya şifre." });
            }
            
            if (string.IsNullOrWhiteSpace(request.UserIp) || request.Amount <= 0 || request.UserBasket == null || !request.UserBasket.Any())
            {
                return BadRequest(new { result = "fail", message = "Geçersiz istek. Gerekli alanlar eksik veya hatalı." });
            }
            
            var merchantId = _paytrSettings.MerchantId;
            var publicKey = _paytrSettings.PublicKey;
            var privateKey = _paytrSettings.PrivateKey;
            var merchantSalt = _paytrSettings.MerchantSalt;
            var merchantKey = _paytrSettings.MerchantKey;

            if (string.IsNullOrWhiteSpace(merchantId) ||
                string.IsNullOrWhiteSpace(publicKey) ||
                string.IsNullOrWhiteSpace(privateKey) ||
                string.IsNullOrWhiteSpace(merchantSalt))
            {
                return StatusCode(500, new { result = "fail", message = "Sunucu yapılandırması eksik." });
            }

            var token = await _authService.GetCachedTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                var paytrToken = Utils.CryptoHelper.GeneratePaytrToken(
                    merchantId, publicKey, privateKey, merchantSalt, merchantKey
                );

                var loginResponse = await _authService.LoginAsync(merchantId, publicKey, paytrToken);
                if (loginResponse.Result != "success")
                {
                    return BadRequest(new { result = "fail", message = loginResponse.Message });
                }

                token = loginResponse.AccessToken;
                await _authService.CacheTokenAsync(token, TimeSpan.FromMinutes(5));
            }
            
            var paymentResponse = await _paymentService.StartPaymentAsync(request, token);
            return Ok(paymentResponse);
        }
    }
}
