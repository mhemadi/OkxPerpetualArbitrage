using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OkxPerpetualArbitrage.Application.Contracts.BackgroundService;
using OkxPerpetualArbitrage.Application.Contracts.Logic;

namespace OkxPerpetualArbitrage.Application.Services.BackgroundTaskServices
{
    /// <summary>
    /// This service processes open requests in the OpenPositionProcessingChannel and opens the symbols requested
    /// </summary>
    public class PositionOpener : IPositionOpener
    {
        private readonly ILogger<PositionOpener> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IOpenPositionProcessingChannel _openPositionProcessingChannel;
        private IPositionOpenCloseLogic _positionOpenCloseLogic;

        public PositionOpener(ILogger<PositionOpener> logger, IServiceProvider serviceProvider, IOpenPositionProcessingChannel openPositionProcessingChannel)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _openPositionProcessingChannel = openPositionProcessingChannel;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            await foreach (var dto in _openPositionProcessingChannel.ReadAllAsync(stoppingToken))
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    _positionOpenCloseLogic = scope.ServiceProvider.GetRequiredService<IPositionOpenCloseLogic>();
                    try
                    {
                        await _positionOpenCloseLogic.Open(dto.Symbol, dto.PositionDemandId, dto.LotSize, dto.LotSizeChunck, dto.MinSpread, dto.PotentialPosition);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process open demand {@OpenPositionProcessingChannelDto}", dto);
                    }

                }
            }
        }
    }
}
