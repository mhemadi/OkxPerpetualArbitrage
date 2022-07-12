namespace OkxPerpetualArbitrage.Application.Models.DTOs
{
    public class CurrentPositionListItemDto
    {
        public string Symbol { get; set; }
        public decimal PositionSize { get; set; }
        public decimal PositionValue { get; set; }
        public decimal CurrentFunding { get; set; }
        public decimal NextFunding { get; set; }
        public decimal TotalFundingIncome { get; set; }
        public decimal TotalFees { get; set; }
        public decimal EstimatedCloseCost { get; set; }
        public decimal PNL { get; set; }
        public bool CloseInProgress { get; set; }
        public string Error { get; set; }
        public DateTime OpenDate { get; set; }
    }
}
