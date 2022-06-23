namespace OkxPerpetualArbitrage.BlazorUIExample.Models
{
    public class OpenRequestDto
    {
        public string Symbol { get; set; }
        public decimal Size { get; set; }
        public decimal MinSpread { get; set; }
    }
}
