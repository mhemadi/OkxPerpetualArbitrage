using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Exceptions;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Services
{


    public class GetInProgressDemandsLogic : IGetInProgressDemandsLogic
    {
        private readonly IPositionDemandRepository _positionDemandRepository;

        public GetInProgressDemandsLogic(IPositionDemandRepository positionDemandRepository)
        {
            _positionDemandRepository = positionDemandRepository;

        }

        public async Task<PositionDemand> GetInProggressDemand(string symbol)
        {
            var r = await _positionDemandRepository.GetInProgressDemandsBySymbol(symbol);
            if (r == null || r.Count == 0)
                return null;
            if (r.Count > 1)
                throw new OkxPerpetualArbitrageCustomException("There are multiple inprogress demands for the symbol " + symbol);
            return r[0];
        }
    }
}
