using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IOrderCreateLogic
    {
        Task<string> CreateAdditionalSpotOrder(string symbol, PotentialPosition pp, decimal spotSize, bool tryToBeMaker, bool open);
        Task<Tuple<string, decimal>> CreateFirstSpotOrder(string symbol, decimal perpOrderSize, PotentialPosition pp, bool tryToBeMaker, bool open);
        Task<string> CreatePerpOrder(string symbol, decimal spreadThreshold, decimal size, PotentialPosition pp, bool tryToBeMaker, bool open);
    }
}
