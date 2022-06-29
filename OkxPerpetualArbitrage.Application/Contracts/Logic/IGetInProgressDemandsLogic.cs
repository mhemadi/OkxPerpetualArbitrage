using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IGetInProgressDemandsLogic
    {
        Task<PositionDemand> GetInProggressDemand(string symbol);
    }
}
