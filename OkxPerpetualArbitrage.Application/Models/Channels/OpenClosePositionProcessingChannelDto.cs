using OkxPerpetualArbitrage.Domain.Entities;

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
