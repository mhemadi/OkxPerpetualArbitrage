namespace OkxPerpetualArbitrage.BlazorUIExample.Models
{
    public class CloseRequestDto
    {
        public string Symbol { get; set; }
        public decimal Size { get; set; }
        public decimal MinSpread { get; set; }
        public bool IsInstant { get; set; }
    }
}
