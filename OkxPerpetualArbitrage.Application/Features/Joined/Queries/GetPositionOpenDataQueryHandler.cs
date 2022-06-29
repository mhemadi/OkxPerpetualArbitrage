using AutoMapper;
using MediatR;
using OkxPerpetualArbitrage.Application.Contracts.OkxApi;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Features.Joined.Queries
{
    /// <summary>
    /// Handles the request to the get data regarding a specific position
    /// The data is presented to the user prior to opening a position
    /// </summary>
    public class GetPositionOpenDataQueryHandler : IRequestHandler<GetPositionOpenDataQuery, PositionDataDto>
    {
        private readonly IInProgressDemandLogic _inProgressDemandLogic;
        private readonly IOkxApiLogic _okxApiLogic;
        private readonly IPotentialPositionProcessorLogic _potentialPositionProcessorLogic;

        public GetPositionOpenDataQueryHandler(IInProgressDemandLogic inProgressDemandLogic,
            IOkxApiLogic okxApiLogic, IPotentialPositionProcessorLogic potentialPositionProcessorLogic)
        {
            _inProgressDemandLogic = inProgressDemandLogic;
           _okxApiLogic = okxApiLogic;
            _potentialPositionProcessorLogic = potentialPositionProcessorLogic;
        }
        public async Task<PositionDataDto> Handle(GetPositionOpenDataQuery request, CancellationToken cancellationToken)
        {
            var pp = await _potentialPositionProcessorLogic.GetPotentialPosition(request.Symbol);
            var inProfressFunds = await _inProgressDemandLogic.GetTotalInProgressRequiredFunds();
            var balance = await _okxApiLogic.GetAvailableUsdtBalance();
            var available = (balance - inProfressFunds) * 0.97m;
            var maxSize = Math.Floor(available / pp.MarkPrice / pp.ContractValuePerp) * pp.ContractValuePerp;
            var dto = new PositionDataDto() { Symbol = request.Symbol, MaxSize = maxSize, Spread = pp.Spread };
            return dto;
        }
    }
}
