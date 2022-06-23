using AutoMapper;
using MediatR;
using OkxPerpetualArbitrage.Application.Contracts.ApiService;
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
        private readonly IOrderFillRepository _orderFillRepository;
        private readonly IPotentialPositionRepository _potentialPositionRepository;
        private readonly IApiService _apiService;
        private readonly IInProgressDemandLogic _inProgressDemandLogic;

        public GetPositionOpenDataQueryHandler(IOrderFillRepository orderFillRepository, IPotentialPositionRepository potentialPositionRepository
            , IApiService apiService, IInProgressDemandLogic inProgressDemandLogic)
        {
            _orderFillRepository = orderFillRepository;
            _potentialPositionRepository = potentialPositionRepository;
            _apiService = apiService;
            _inProgressDemandLogic = inProgressDemandLogic;
        }
        public async Task<PositionDataDto> Handle(GetPositionOpenDataQuery request, CancellationToken cancellationToken)
        {
            var pp = await _potentialPositionRepository.GetPotentialPosition(request.Symbol);
            var inProfressFunds = await _inProgressDemandLogic.GetTotalInProgressRequiredFunds();
            var balance = await _apiService.GetUSDTBalance();
            var available = (balance.Available - inProfressFunds) * 0.97m;
            var maxSize = Math.Floor(available / pp.MarkPrice / pp.ContractValuePerp) * pp.ContractValuePerp;
            var dto = new PositionDataDto() { Symbol = request.Symbol, MaxSize = maxSize, Spread = pp.Spread };
            return dto;
        }

    }
}
