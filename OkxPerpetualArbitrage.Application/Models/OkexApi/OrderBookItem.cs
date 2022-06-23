using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Application.Models.OkexApi
{
    public class OrderBookItem
    {
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }
}
