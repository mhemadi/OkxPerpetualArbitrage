using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Models.Channels
{
    public class OpenClosePositionProcessingChannelDto
    {
        public string Symbol { get; set; }
        public int PositionDemandId { get; set; }
        public decimal LotSize { get; set; }
        public decimal LotSizeChunck { get; set; }
        public decimal MinSpread { get; set; }
        public PotentialPosition PotentialPosition { get; set; }
    }
}
