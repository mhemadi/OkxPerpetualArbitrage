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

    public class PotentialPositionProcessorLogic : IPotentialPositionProcessorLogic
    {
        private readonly IPotentialPositionRepository _potentialPositionRepository;

        public PotentialPositionProcessorLogic(IPotentialPositionRepository potentialPositionRepository)
        {
            _potentialPositionRepository = potentialPositionRepository;
        }

        public async Task<PotentialPosition> GetPotentialPosition(string symbol)
        {
            var pp = await _potentialPositionRepository.GetPotentialPosition(symbol);
            if (pp == null)
                throw new OkxPerpetualArbitrageCustomException("PotentnialPosition was not found");
            return pp;
        }

        public async Task<List<PotentialPosition>> GetAllPotentialPositions()
        {
            var pps = await _potentialPositionRepository.GetAllAsync();
            return pps;
        }
    }
}
