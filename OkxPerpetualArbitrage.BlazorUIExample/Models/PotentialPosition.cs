namespace OkxPerpetualArbitrage.BlazorUIExample.Models
{
    public class PotentialPosition
    {
        public string Symbol { get; set; }
        public decimal MarkPrice { get; set; }

        public decimal Funding { get; set; }

        public decimal NextFunding { get; set; }

        public decimal Spread { get; set; }

        public decimal Rating { get; set; }

        public bool InProgress { get; set; }
    }
}
