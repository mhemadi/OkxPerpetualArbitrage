using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OkxPerpetualArbitrage.Application.Contracts.BackgroundService;
using OkxPerpetualArbitrage.Application.Contracts.Logic;

namespace OkxPerpetualArbitrage.Application.Services.BackgroundTaskServices
{
    public class PositionCloser : IPositionCloser
    {
        private readonly ILogger<PositionCloser> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IClosePositionProcessingChannel _closerPositionProcessingChannel;
        private IPositionOpenCloseLogic _positionOpenCloseLogic;

        public PositionCloser(ILogger<PositionCloser> logger, IServiceProvider serviceProvider, IClosePositionProcessingChannel closePositionProcessingChannel)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _closerPositionProcessingChannel = closePositionProcessingChannel;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var dto in _closerPositionProcessingChannel.ReadAllAsync(stoppingToken))
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    _positionOpenCloseLogic = scope.ServiceProvider.GetRequiredService<IPositionOpenCloseLogic>();
                    try
                    {
                        await _positionOpenCloseLogic.Close(dto.Symbol, dto.PositionDemandId, dto.LotSize, dto.LotSizeChunck, dto.MinSpread, dto.PotentialPosition, dto.IsInstant);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process close demand {@OpenPositionProcessingChannelDto}", dto);
                    }

                }
            }
        }
    }
}
