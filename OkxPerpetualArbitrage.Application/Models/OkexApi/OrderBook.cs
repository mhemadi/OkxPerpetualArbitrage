namespace OkxPerpetualArbitrage.Application.Models.OkexApi
{
    public class OrderBook
    {
        public string Market { get; set; }
        public List<OrderBookItem> Bids { get; set; }
        public List<OrderBookItem> Asks { get; set; }
    }
}
