using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Services
{
    public class PositionManagerLogic : IPositionOpenCloseLogic
    {
        private readonly IPositionOpenLogic _positionOpenLogic;
        private readonly IPositionCloseLogic _positionCloseLogic;

        public PositionManagerLogic(IPositionOpenLogic positionOpenLogic, IPositionCloseLogic positionCloseLogic)
        {
            _positionOpenLogic = positionOpenLogic;
            _positionCloseLogic = positionCloseLogic;
        }

        public async Task Close(string symbol, int positionDemandId, decimal lotSize, decimal lotSizeChunk, decimal minSpread, PotentialPosition potentialPosition, bool isInstant)
        {
           await _positionCloseLogic.Close(symbol, positionDemandId, lotSize, lotSizeChunk, minSpread, potentialPosition, isInstant);
        }

        public async Task Open(string symbol, int positionDemandId, decimal lotSize, decimal lotSizeChunk, decimal minSpread, PotentialPosition potentialPosition)
        {
            await _positionOpenLogic.Open(symbol, positionDemandId, lotSize, lotSizeChunk, minSpread, potentialPosition);
        }
    }
}
