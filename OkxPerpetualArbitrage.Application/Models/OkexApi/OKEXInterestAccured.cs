using OkxPerpetualArbitrage.Application.Models.OkexApi.Enums;

namespace OkxPerpetualArbitrage.Application.Models.OkexApi
{
    public class OKEXInterestAccured
    {
        public string Currency { get; set; }
        public string Instrument { get; set; }
        public decimal Interest { get; set; }
        public decimal InterestRateHourly { get; set; }
        public decimal Liability { get; set; }
        public OKEXTadeMode MarginMode { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
