using MediatR;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Features.Joined.Queries
{
    /// <summary>
    /// Handles the request to the get data regarding a specific position
    /// The data is presented to the user prior to closing a position
    /// </summary>
    public class GetPositionCloseDataQueryHandler : IRequestHandler<GetPositionCloseDataQuery, PositionDataDto>
    {
        private readonly IPotentialPositionProcessorLogic _potentialPositionProcessorLogic;
        private readonly ITotalAvailableCloseSizeCalculatorLogic _totalAvailableCloseSizeCalculator;

        public GetPositionCloseDataQueryHandler(IPotentialPositionProcessorLogic potentialPositionProcessorLogic, ITotalAvailableCloseSizeCalculatorLogic totalAvailableCloseSizeCalculator)
        {
            _potentialPositionProcessorLogic = potentialPositionProcessorLogic;
            _totalAvailableCloseSizeCalculator = totalAvailableCloseSizeCalculator;
        }
        public async Task<PositionDataDto> Handle(GetPositionCloseDataQuery request, CancellationToken cancellationToken)
        {
            var maxSize = await _totalAvailableCloseSizeCalculator.GetTotalAvailableSize(request.Symbol);
            var pp = await _potentialPositionProcessorLogic.GetPotentialPosition(request.Symbol);
            var dto = new PositionDataDto() { Symbol = request.Symbol, MaxSize = maxSize, Spread = pp.CloseSpread };
            return dto;
        }


    }
}
