using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Models.DTOs
{
    public class PotentialPositionListItemDto
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
