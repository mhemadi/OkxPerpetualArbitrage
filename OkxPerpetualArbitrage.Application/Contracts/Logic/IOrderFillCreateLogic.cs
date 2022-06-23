using OkxPerpetualArbitrage.Application.Models.OkexApi;
using OkxPerpetualArbitrage.Domain.Entities;
using OkxPerpetualArbitrage.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IOrderFillCreateLogic
    {
        Task AddOrderFill(OKEXOrder oKEXOrder, int positionDemandId, PartInPosition partInPosition, PotentialPosition potentialPosition);
    }
}
