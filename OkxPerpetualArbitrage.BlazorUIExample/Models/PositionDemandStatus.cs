namespace OkxPerpetualArbitrage.BlazorUIExample.Models
{
    public class PositionDemandStatus
    {
        public int PositionDemandId { get; set; }
        public string Symbol { get; set; }
        public decimal TotalSize { get; set; }
        public decimal Filled { get; set; }
        public bool IsCanceled { get; set; }
        public decimal FilledPercent { get { return TotalSize >0 ? Math.Round(Filled / TotalSize * 100) : 0; } }
    }
}
