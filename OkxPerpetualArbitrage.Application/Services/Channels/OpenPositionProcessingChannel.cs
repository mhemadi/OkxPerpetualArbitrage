using Microsoft.Extensions.Options;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Models.Channels;
using OkxPerpetualArbitrage.Application.Models.InfrastructureSettings;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;

namespace OkxPerpetualArbitrage.Application.Services.Channels
{


    public class OpenPositionProcessingChannel : IOpenPositionProcessingChannel
    {
        private readonly GeneralSetting _settings;
   //     private readonly ILogger _logger;
        private readonly Channel<OpenPositionProcessingChannelDto> _channel;
        public OpenPositionProcessingChannel(IOptions<GeneralSetting> settings)
        {

            _settings = settings.Value;
      //      _logger = logger;
            var o = new BoundedChannelOptions(_settings.MaxOpenDemands) { SingleReader = true, SingleWriter = false };
            _channel = Channel.CreateBounded<OpenPositionProcessingChannelDto>(o);
        }

        public bool AddOpenPositionDemand(OpenPositionProcessingChannelDto openPositionProcessingChannelDto)
        {

            if (_channel.Writer.TryWrite(openPositionProcessingChannelDto))
            {
     //           _logger.LogInformation("Success in writing to open position channel {@openPositionProcessingChannelDto}", openPositionProcessingChannelDto);
                return true;
            }

            return false;
        }

        public IAsyncEnumerable<OpenPositionProcessingChannelDto> ReadAllAsync(CancellationToken stoppingToken)
        {
            return _channel.Reader.ReadAllAsync(stoppingToken);
        }

    }
}
