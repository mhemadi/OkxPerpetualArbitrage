using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IGetCurrentPositionsLogic
    {
        Task<List<CurrentPositionListItemDto>> GetCurrentPositions(bool checkError, CancellationToken cancellationToken);
    }
}
