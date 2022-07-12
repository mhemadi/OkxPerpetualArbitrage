using AutoMapper;
using MediatR;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Exceptions;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Features.PositionDemands.Queries
{
    /// <summary>
    /// Handles the request to get the status of an ongoing open or close request
    /// </summary>
    public class GetPositionDemandStatusQueryHandler : IRequestHandler<GetPositionDemandStatusQuery, PositionDemandStatusDto>
    {
        private readonly IGetInProgressDemandsLogic _getInProgressDemandsLogic;
        private readonly IMapper _mapper;

        public GetPositionDemandStatusQueryHandler(IGetInProgressDemandsLogic getInProgressDemandsLogic, IMapper mapper)
        {
            _getInProgressDemandsLogic = getInProgressDemandsLogic;
            _mapper = mapper;
        }

        public async Task<PositionDemandStatusDto> Handle(GetPositionDemandStatusQuery request, CancellationToken cancellationToken)
        {

            var demand = await _getInProgressDemandsLogic.GetInProggressDemand(request.Symbol);
            if (demand == null)
                throw new OkxPerpetualArbitrageCustomException("Can not find the one demand corresponding to the symbol");
            var dto = _mapper.Map<PositionDemandStatusDto>(demand);
            return dto;
        }
    }
}
