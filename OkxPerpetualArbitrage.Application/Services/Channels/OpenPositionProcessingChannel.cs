using Microsoft.Extensions.Options;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Models.Channels;
using OkxPerpetualArbitrage.Application.Models.InfrastructureSettings;
using System.Threading.Channels;

namespace OkxPerpetualArbitrage.Application.Services.Channels
{
    public class OpenPositionProcessingChannel : IOpenPositionProcessingChannel
    {
        private readonly GeneralSetting _settings;
        private readonly Channel<OpenPositionProcessingChannelDto> _channel;
        public OpenPositionProcessingChannel(IOptions<GeneralSetting> settings)
        {
            _settings = settings.Value;
            var o = new BoundedChannelOptions(_settings.MaxOpenDemands) { SingleReader = true, SingleWriter = false };
            _channel = Channel.CreateBounded<OpenPositionProcessingChannelDto>(o);
        }

        public bool AddOpenPositionDemand(OpenPositionProcessingChannelDto openPositionProcessingChannelDto)
        {
            if (_channel.Writer.TryWrite(openPositionProcessingChannelDto))
            {
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
