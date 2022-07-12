using OkxPerpetualArbitrage.Application.Models.OkexApi;
using OkxPerpetualArbitrage.Domain.Entities;
using OkxPerpetualArbitrage.Domain.Entities.Enums;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IOrderFillCreateLogic
    {
        Task AddOrderFill(OKEXOrder oKEXOrder, int positionDemandId, PartInPosition partInPosition, PotentialPosition potentialPosition);
    }
}
