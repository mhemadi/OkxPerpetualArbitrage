using OkxPerpetualArbitrage.Application.Models.OkexApi.Enums;

namespace OkxPerpetualArbitrage.Application.Models.OkexApi
{
    public class OKEXOrder
    {
        public int OKEXOrderId { get; set; }
        public int OKEXPositionId { get; set; }
        public OKEXOrderPositionType OrderPositionType { get; set; }
        public decimal Size { get; set; } //accFillSz
        public decimal InitialSize { get; set; }//sz
        public decimal Price { get; set; }//avgPx
        public decimal InitialPrice { get; set; }//px
        public DateTime CreatedDate { get; set; }//cTime
        public DateTime UpdatedDate { get; set; }//uTime
        public string Category { get; set; }//category
        public string ClientOrderId { get; set; }//clOrdId
        public decimal Fee { get; set; }//fee
        public string FeeCurrency { get; set; }//feeCcy
        public string Instrument { get; set; }//instId
        public OKEXInstrumentType InstrumentType { get; set; }//instType
        public decimal Leverage { get; set; }//lever
        public OKEXOrderType OrderType { get; set; }//ordType
        public OKEXOrderSide OrderSide { get; set; }//side
        public OKEXPostitionSide PostitionSide { get; set; }//posSide
        public string State { get; set; }//state
        public OKEXTadeMode OKEXTadeMode { get; set; }//tdMode
        public string OrderIdOKEX { get; set; }//ordId
        public decimal SpotPrice { get; set; } //Only applies to futureOrders

    }
}
