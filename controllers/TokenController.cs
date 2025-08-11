using Microsoft.AspNetCore.Mvc;
using PaytrAuthApi.Models;
using PaytrAuthApi.Services;
using PaytrAuthApi.Settings;

namespace PaytrAuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly PaytrSettings _paytrSettings;  // tip güvenliği

        public TokenController(AuthService authService, Microsoft.Extensions.Options.IOptions<PaytrSettings> paytrSettings)
        {
            _authService = authService;
            _paytrSettings = paytrSettings.Value;
        }

        [HttpPost("get-token")]
        public async Task<IActionResult> GetToken([FromBody] GetTokenRequest request)
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Unauthorized(new GetTokenResponse
                {
                    DeviceId = request.DeviceId,
                    AccessToken = null,
                    Error = "Authorization header eksik"
                });
            }

            var authHeader = Request.Headers["Authorization"].ToString();
            if (!_authService.ValidateBasicAuth(authHeader))
            {
                return Unauthorized(new GetTokenResponse
                {
                    DeviceId = request.DeviceId,
                    AccessToken = null,
                    Error = "Geçersiz kullanıcı adı veya şifre"
                });
            }

            
            if (request.DeviceId != "5")
            {
                return BadRequest(new GetTokenResponse
                {
                    DeviceId = request.DeviceId,
                    AccessToken = null,
                    Error = "Cihaz bulunamadı"
                });
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
                return StatusCode(500, new GetTokenResponse
                {
                    DeviceId = request.DeviceId,
                    AccessToken = null,
                    Error = "Sunucu yapılandırması eksik"
                });
            }

        
            var paytrToken = Utils.CryptoHelper.GeneratePaytrToken(
                merchantId, publicKey, privateKey, merchantSalt, merchantKey
            );

            Console.WriteLine("Oluşturulan PAYTR Token: " + paytrToken);
            
            var response = await _authService.LoginAsync(merchantId, publicKey, paytrToken);
            if (response.Result == "success")
            {
                return Ok(new GetTokenResponse
                {
                    DeviceId = request.DeviceId,
                    AccessToken = response.AccessToken,
                    Error = null
                });
            }
            else
            {
                return BadRequest(new GetTokenResponse
                {
                    DeviceId = request.DeviceId,
                    AccessToken = null,
                    Error = response.Message
                });
            }
        }
    }
}
