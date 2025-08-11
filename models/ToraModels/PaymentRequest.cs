public class BasketItem
{
    public string Name { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class PaymentRequestModel
{
    public string UserIp { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public List<BasketItem> UserBasket { get; set; } = new();
}