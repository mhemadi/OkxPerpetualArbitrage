using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IGetPotentialPositionsLogic
    {
        Task<List<PotentialPositionListItemDto>> GetPotentialPositions(CancellationToken cancellationToken);
    }
}
