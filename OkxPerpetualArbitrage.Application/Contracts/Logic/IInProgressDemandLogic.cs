using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IInProgressDemandLogic
    {
        Task<decimal> GetTotalInProgressRequiredFunds();
    }
}
