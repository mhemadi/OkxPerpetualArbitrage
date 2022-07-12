namespace OkxPerpetualArbitrage.Application.Models.OkexApi
{
    public class OKEXBalance
    {
        public string Symbol { get; set; }
        public decimal Equity { get; set; }
        public decimal Cash { get; set; }
        public decimal Available { get { return Equity < Cash ? Equity : Cash; } } //Return the smaller amount
    }
}
