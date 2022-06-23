using OkxPerpetualArbitrage.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Domain.Entities
{
    public class OrderFill
    {
        public int OrderFillId { get; set; }
        public string ClientOrderId { get; set; }
        public decimal Size { get; set; }
        public decimal Lot { get; set; }
        public decimal Filled { get; set; }
        public decimal Price { get; set; }
        public decimal Fee { get; set; }
        public string FeeCurrency { get; set; }
        public DateTime TimeStamp { get; set; }
        public PartInPosition PartInPosition { get; set; }
        public PositionDemand PositionDemand { get; set; }

    }
}
