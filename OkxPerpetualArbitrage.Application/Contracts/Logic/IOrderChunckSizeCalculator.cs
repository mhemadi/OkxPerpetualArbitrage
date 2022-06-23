using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IPositionChunckSizeCalculator
    {
        decimal GetChunkLotSize(PotentialPosition pp, decimal lotSize);
    }
}
