using OkxPerpetualArbitrage.Application.Models.Channels;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IClosePositionProcessingChannel
    {
        bool AddClosePositionDemand(ClosePositionProcessingChannelDto closePositionProcessingChannelDto);
        IAsyncEnumerable<ClosePositionProcessingChannelDto> ReadAllAsync(CancellationToken stoppingToken);
    }
}
