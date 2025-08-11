public class PaymentRequestBodyModel
{
    public string UserIp { get; set; } = string.Empty;
    public string MerchantOid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int PaymentAmount { get; set; }
    public string Callback { get; set; } = string.Empty;
    public int TestMode { get; set; }
    public List<BasketItemRequest> UserBasket { get; set; } = new();
}