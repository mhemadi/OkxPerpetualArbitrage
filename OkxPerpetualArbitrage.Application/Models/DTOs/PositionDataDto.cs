namespace OkxPerpetualArbitrage.Application.Models.DTOs
{
    public class PositionDataDto
    {
        public string Symbol { get; set; }
        public decimal MaxSize { get; set; }
        public decimal Spread { get; set; }
    }
}
