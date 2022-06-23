using OkxPerpetualArbitrage.Application.Models.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IClosePositionProcessingChannel
    {
        bool AddClosePositionDemand(ClosePositionProcessingChannelDto closePositionProcessingChannelDto);
        IAsyncEnumerable<ClosePositionProcessingChannelDto> ReadAllAsync(CancellationToken stoppingToken);
    }
}
