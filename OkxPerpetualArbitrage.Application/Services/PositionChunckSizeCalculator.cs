using Microsoft.Extensions.Options;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Models.InfrastructureSettings;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Application.Services
{
 

    public class PositionChunckSizeCalculator : IPositionChunckSizeCalculator
    {
        private readonly GeneralSetting _setting;
        public PositionChunckSizeCalculator(IOptions<GeneralSetting> setting)
        {
            _setting = setting.Value;
        }

        public decimal GetChunkLotSize(PotentialPosition pp, decimal lotSize)
        {
            decimal lotValue = pp.ContractValuePerp * pp.MarkPrice;
            if (lotValue > _setting.ChuncDollarkValue)
                return 1;
            var chunkLotSize = Math.Round(_setting.ChuncDollarkValue / lotValue);
            if (chunkLotSize > lotSize)
                chunkLotSize = lotSize;
            return chunkLotSize;
        }
    }
}
