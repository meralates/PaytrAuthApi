namespace PaytrAuthApi.Models
{
    public class GetTokenResponse
    {
        public string DeviceId { get; set; } = string.Empty;
        public string? AccessToken { get; set; }
        public string? Error { get; set; } 
    }
}
