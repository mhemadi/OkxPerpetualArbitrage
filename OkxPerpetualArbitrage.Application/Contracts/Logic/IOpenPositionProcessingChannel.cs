using OkxPerpetualArbitrage.Application.Models.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IOpenPositionProcessingChannel
    {
        bool AddOpenPositionDemand(OpenPositionProcessingChannelDto openPositionProcessingChannelDto);
        IAsyncEnumerable<OpenPositionProcessingChannelDto> ReadAllAsync(CancellationToken stoppingToken);
    }
}
