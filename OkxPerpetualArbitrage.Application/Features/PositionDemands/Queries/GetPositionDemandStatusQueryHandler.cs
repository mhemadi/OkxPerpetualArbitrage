using AutoMapper;
using MediatR;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Exceptions;
using OkxPerpetualArbitrage.Application.Features.PositionDemands.Queries;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using OkxPerpetualArbitrage.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Features.PositionDemands.Queries
{
    /// <summary>
    /// Handles the request to get the status of an ongoing open or close request
    /// </summary>
    public class GetPositionDemandStatusQueryHandler : IRequestHandler<GetPositionDemandStatusQuery, PositionDemandStatusDto>
    {
        private readonly IPositionDemandRepository _positionDemandRepository;
        private readonly IMapper _mapper;

        public GetPositionDemandStatusQueryHandler(IPositionDemandRepository positionDemandRepository, IMapper mapper)
        {
            _positionDemandRepository = positionDemandRepository;
            _mapper = mapper;
        }

        public async Task<PositionDemandStatusDto> Handle(GetPositionDemandStatusQuery request, CancellationToken cancellationToken)
        {
            var r = await _positionDemandRepository.GetInProgressDemandsBySymbol(request.Symbol);
            if (r == null || r.Count != 1) //dont throw
                throw new OkxPerpetualArbitrageCustomException("Can not find the one demand corresponding to the symbol");
            var req = r[0];
            var dto = _mapper.Map<PositionDemandStatusDto>(req);
            return dto;
        }
    }
}
