namespace PaytrAuthApi.Models
{
    public class PaymentResponse
    {
        public string result { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public PaymentData? data { get; set; }
    }

    public class PaymentData
    {
        public string token { get; set; } = string.Empty;
    }
}