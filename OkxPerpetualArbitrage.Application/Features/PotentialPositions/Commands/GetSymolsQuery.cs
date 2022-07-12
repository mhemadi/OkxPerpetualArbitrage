using MediatR;

namespace OkxPerpetualArbitrage.Application.Features.PotentialPositions.Commands
{
    public class GetSymolsQuery : IRequest<List<string>>
    {
    }
}
