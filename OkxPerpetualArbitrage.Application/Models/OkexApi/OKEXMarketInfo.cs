using OkxPerpetualArbitrage.Application.Models.OkexApi.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Application.Models.OkexApi
{
    public class OKEXMarketInfo
    {
        public string Instrument { get; set; }
        public decimal SpotAsk { get; set; }
        public decimal SpotBid { get; set; }
        public decimal PerpAsk { get; set; }
        public decimal PerpBid { get; set; }
        public OKEXInstrumentType InstrumentType { get; set; }
    }
}
