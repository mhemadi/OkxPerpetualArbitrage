using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Application.Models.OkexApi
{
    public class OKEXBill
    {
        public string BillId { get; set; }
        public string Currency { get; set; }
        public decimal BalanceChange { get; set; }
        public string Instrument { get; set; }
        public DateTime TimeStamp { get; set; }
        public string BillType { get; set; }
    }
}
