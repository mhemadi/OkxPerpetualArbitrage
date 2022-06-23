using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Domain.Entities
{
    public class FundingIncome
    {
        public int FundingIncomeId { get; set; }
        public string BillId { get; set; }
        public string Symbol { get; set; }
        public DateTime TimeStamp { get; set; }
        public decimal Amount { get; set; }
        public bool IsInCurrentPosition { get; set; }
    }
}
