using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PaytrAuthApi.Models;

namespace PaytrAuthApi.Services
{
    
    public class PaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly NeoPosSettings _settings;

        public PaymentService(HttpClient httpClient, IOptions<NeoPosSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task<PaymentResponse> StartPaymentAsync(PaymentRequestModel model, string accessToken)
        {
            var requestBody = new
            {
                user_ip = model.UserIp,
                merchant_oid = Guid.NewGuid().ToString("N"),
                email = "meralates0@outlook.com",
                payment_amount = (int)(model.Amount * 100),
                callback = "https://heryerdeyika.com/",
                test_mode = 0,
                user_basket = model.UserBasket.Select(item => new object[]
                {
                    item.Name,
                    item.Price,
                    item.Quantity
                }).ToArray()
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var apiUrl = "https://sirius-4x.paytr.com/sdk/neopos/payment";

            try
            {
                var response = await _httpClient.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var responseObject = JsonSerializer.Deserialize<PaymentResponse>(responseContent);

                return responseObject ?? new PaymentResponse
                {
                    result = "fail",
                    message = "Yanıt deserialize edilemedi.",
                    data = null
                };
            }
            catch (Exception ex)
            {
                return new PaymentResponse
                {
                    result = "fail",
                    message = "Ödeme başlatma sırasında bir hata oluştu.",
                    data = new PaymentData { token = ex.Message }
                };
            }
        }

    }
}
