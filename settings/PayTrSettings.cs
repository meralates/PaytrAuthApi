namespace PaytrAuthApi.Settings
{
    public class PaytrSettings
    {
        public string MerchantId { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string PrivateKey { get; set; } = string.Empty;
        public string MerchantSalt { get; set; } = string.Empty;
        
        public string MerchantKey { get; set; } = string.Empty;
    }
}