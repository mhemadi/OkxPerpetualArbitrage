using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Domain.Entities
{
    public class PotentialPositionRatingHistory
    {

        public int PotentialPositionRatingHistoryId { get; set; }
        public string Symbol { get; set; }
        public decimal Rating { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
