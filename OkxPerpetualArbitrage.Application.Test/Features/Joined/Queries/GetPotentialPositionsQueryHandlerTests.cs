using Moq;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Features.Joined.Queries;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OkxPerpetualArbitrage.Application.Test.Features.Joined.Queries
{
    public class GetPotentialPositionsQueryHandlerTests
    {
        [Fact]
        public void GetPotentialPositionsCallsGetPotentialPositions()
        {
            Mock<IGetPotentialPositionsLogic> service = new();
            service.Setup(x => x.GetPotentialPositions(CancellationToken.None))
                .Returns(Task.FromResult(new List<PotentialPositionListItemDto>()));
            GetPotentialPositionsQueryHandler handler = new GetPotentialPositionsQueryHandler(service.Object);

            handler.Handle(
                new GetPotentialPositionsQuery() , CancellationToken.None);

            service.Verify(x => x.GetPotentialPositions(CancellationToken.None)
                , Times.Once);


        }
    }
}
