using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IPositionCheckLogic
    {
        IOrderFillRepository OrderFillRepository { get; }

        Task Checkposition(string symbol, PotentialPosition pp);
    }
}
