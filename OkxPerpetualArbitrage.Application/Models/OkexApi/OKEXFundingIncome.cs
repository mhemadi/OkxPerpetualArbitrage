using OkxPerpetualArbitrage.Application.Models.OkexApi.Enums;

namespace OkxPerpetualArbitrage.Application.Models.OkexApi
{
    public class OKEXFundingIncome
    {
        public int OKEXFundingIncomeId { get; set; }
        public long BillId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public string Instrument { get; set; }
        public OKEXTadeMode MarginMode { get; set; }
    }
}
