using OkxPerpetualArbitrage.Application.Models.Channels;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IOpenPositionProcessingChannel
    {
        bool AddOpenPositionDemand(OpenPositionProcessingChannelDto openPositionProcessingChannelDto);
        IAsyncEnumerable<OpenPositionProcessingChannelDto> ReadAllAsync(CancellationToken stoppingToken);
    }
}
