namespace OkxPerpetualArbitrage.Application.Models.Channels
{
    public class ClosePositionProcessingChannelDto : OpenClosePositionProcessingChannelDto
    {
        public bool IsInstant { get; set; }
    }
}
