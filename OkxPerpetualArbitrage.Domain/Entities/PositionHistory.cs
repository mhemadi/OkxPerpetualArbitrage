namespace OkxPerpetualArbitrage.Domain.Entities
{
    public class PositionHistory
    {
        public int PositionHistoryId { get; set; }
        public string Symbol { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime CloseDate { get; set; }
        public decimal SpotTrade { get; set; }
        public decimal PerpTrade { get; set; }
        public decimal Fee { get; set; }
        public decimal Funding { get; set; }
        public decimal TotalPNL { get; set; }
    }
}
