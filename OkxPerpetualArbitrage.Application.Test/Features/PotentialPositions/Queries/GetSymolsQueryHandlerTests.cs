using Moq;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Features.PotentialPositions.Commands;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OkxPerpetualArbitrage.Application.Test.Features.PotentialPositions.Queries
{
    public class GetSymolsQueryHandlerTests
    {
        [Fact]
        public async void GetSymolsQueryCallsGetAllPotentialPositions()
        {
            string symbol = "FIL";
            Mock<IPotentialPositionProcessorLogic> service = new();
            service.Setup(x => x.GetAllPotentialPositions())
                .Returns(Task.FromResult(new List<PotentialPosition>() { new PotentialPosition() { Symbol = "B" }
                , new PotentialPosition(){ Symbol = "A" }
             , new PotentialPosition(){ Symbol = "C" }
                }));
            GetSymolsQueryHandler handler = new GetSymolsQueryHandler(service.Object);

            var result = await handler.Handle(
                new GetSymolsQuery(), CancellationToken.None);

            service.Verify(x => x.GetAllPotentialPositions()
                , Times.Once);

            Assert.Equal("A", result[0]);
            Assert.NotEqual("A", result[2]);

        }
    }
}
