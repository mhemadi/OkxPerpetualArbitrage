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
    public class GetCurrentPositionsQueryHandlerTests
    {
        [Fact]
        public void GetCurrentPositionsCallsGetCurrentPositions()
        {
            bool checkError = true;
            Mock<IGetCurrentPositionsLogic> service = new();
            service.Setup(x => x.GetCurrentPositions(checkError, CancellationToken.None))
                .Returns(Task.FromResult(new List<CurrentPositionListItemDto>()));
            GetCurrentPositionsQueryHandler handler = new GetCurrentPositionsQueryHandler(service.Object);

            handler.Handle(
                new GetCurrentPositionsQuery() { CheckError = checkError   }, CancellationToken.None);

            service.Verify(x => x.GetCurrentPositions(true, CancellationToken.None)
                , Times.Once);
            service.Verify(x => x.GetCurrentPositions(false, CancellationToken.None)
                , Times.Never);

        }
    }
}
