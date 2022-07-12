namespace OkxPerpetualArbitrage.Domain.Entities
{
    public class PotentialPosition
    {
        public int PotentialPositionId { get; set; }
        public string Symbol { get; set; }
        public decimal Funding { get; set; }
        public decimal NextFunding { get; set; }
        public decimal Spread { get; set; }
        public decimal CloseSpread { get; set; }
        public decimal Rating { get; set; }
        public decimal MinSizeSpot { get; set; }
        public decimal LotSizeSpot { get; set; }
        public decimal TickSizeSpot { get; set; }
        public decimal MinSizePerp { get; set; }
        public decimal TickSizePerp { get; set; }
        public decimal ContractValuePerp { get; set; }
        public decimal FeeSpotMaker { get; set; }
        public decimal FeeSpotTaker { get; set; }
        public decimal FeePerpMaker { get; set; }
        public decimal FeePerpTaker { get; set; }
        public decimal MarkPrice { get; set; }
        public bool IsUpToDate { get; set; }
        public decimal FundingRateAverageThreeDays { get; set; }
        public decimal FundingRateAverageSevenDays { get; set; }
        public decimal FundingRateAverageFourteenDays { get; set; }


    }
}
