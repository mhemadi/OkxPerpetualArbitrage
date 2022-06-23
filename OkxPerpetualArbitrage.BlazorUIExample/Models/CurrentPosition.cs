namespace OkxPerpetualArbitrage.BlazorUIExample.Models
{
    public class CurrentPosition
    {
        public string Symbol { get; set; }
        public decimal PositionSize { get; set; }
        public decimal PositionValue { get; set; }
        public decimal CurrentFunding { get; set; }
        public decimal NextFunding { get; set; }
        public decimal CurrentFundingPercent { get => CurrentFunding * 100 / PositionValue; }
        public decimal NextFundingPercent { get => NextFunding * 100 / PositionValue; }
        public decimal TotalFundingIncome { get; set; }
        public decimal TotalFees { get; set; }
        public decimal EstimatedCloseCost { get; set; }
        public decimal PNL { get; set; }
        public bool CloseInProgress { get; set; }
        public string Error { get; set; }
    }
}
