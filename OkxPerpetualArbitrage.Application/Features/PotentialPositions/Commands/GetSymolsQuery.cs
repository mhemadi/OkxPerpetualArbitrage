using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Features.PotentialPositions.Commands
{
    public class GetSymolsQuery : IRequest<List<string>>
    {
    }
}
