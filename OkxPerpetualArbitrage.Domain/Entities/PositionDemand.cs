using OkxPerpetualArbitrage.Domain.Entities.Enums;

namespace OkxPerpetualArbitrage.Domain.Entities
{
    public class PositionDemand
    {
        public int PositionDemandId { get; set; }
        public string Symbol { get; set; }
        public PositionDemandState PositionDemandState { get; set; }
        public decimal TotalSize { get; set; }
        public decimal Filled { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public decimal Spread { get; set; }
        public decimal ActualSpread { get; set; }
        public PositionDemandSide PositionDemandSide { get; set; }
        public bool IsInstant { get; set; }
        public bool IsCanceled { get; set; }
    }
}
