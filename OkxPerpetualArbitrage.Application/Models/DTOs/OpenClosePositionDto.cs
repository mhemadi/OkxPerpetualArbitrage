namespace OkxPerpetualArbitrage.Application.Models.DTOs
{
    public class OpenClosePositionDto
    {
        public string Symbol { get; set; }
        public decimal Size { get; set; }
        public decimal MinSpread { get; set; }
    }
}
