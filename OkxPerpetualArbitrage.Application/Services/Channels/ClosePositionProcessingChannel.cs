using Microsoft.Extensions.Options;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Models.Channels;
using OkxPerpetualArbitrage.Application.Models.InfrastructureSettings;
using System.Threading.Channels;

namespace OkxPerpetualArbitrage.Application.Services.Channels
{


    public class ClosePositionProcessingChannel : IClosePositionProcessingChannel
    {
        private readonly GeneralSetting _settings;
        private readonly Channel<ClosePositionProcessingChannelDto> _channel;
        public ClosePositionProcessingChannel(IOptions<GeneralSetting> settings)
        {
            _settings = settings.Value;
            var o = new BoundedChannelOptions(_settings.MaxOpenDemands) { SingleReader = true, SingleWriter = false };
            _channel = Channel.CreateBounded<ClosePositionProcessingChannelDto>(o);
        }

        public bool AddClosePositionDemand(ClosePositionProcessingChannelDto closePositionProcessingChannelDto)
        {

            if (_channel.Writer.TryWrite(closePositionProcessingChannelDto))
            {
                return true;
            }

            return false;
        }

        public IAsyncEnumerable<ClosePositionProcessingChannelDto> ReadAllAsync(CancellationToken stoppingToken)
        {
            return _channel.Reader.ReadAllAsync(stoppingToken);
        }

    }
}
