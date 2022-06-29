using Moq;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Features.Joined.Queries;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OkxPerpetualArbitrage.Application.Test.Features.Joined.Queries
{
    public class GetPositionCloseDataQueryHandlerTests
    {
        [Fact]
        public async void GetPositionCloseDataCallsGetTotalAvailableSizeAndGetPotentialPosition()
        {
          //  bool checkError = true;
            string symbol = "FIL";
            Mock<IPotentialPositionProcessorLogic> service1 = new();
            Mock<ITotalAvailableCloseSizeCalculatorLogic> service2 = new();
            service1.Setup(x => x.GetPotentialPosition(symbol))
                .Returns(Task.FromResult(new PotentialPosition() { CloseSpread = 0.1m }));
            service2.Setup(x => x.GetTotalAvailableSize(symbol))
                .Returns(Task.FromResult(100m));
            GetPositionCloseDataQueryHandler handler = new GetPositionCloseDataQueryHandler(service1.Object, service2.Object);

           var result = await handler.Handle(
                new GetPositionCloseDataQuery() { Symbol = symbol }, CancellationToken.None);

            service1.Verify(x => x.GetPotentialPosition(symbol)
                , Times.Once);
            service1.Verify(x => x.GetPotentialPosition("")
               , Times.Never);
            service2.Verify(x => x.GetTotalAvailableSize(symbol)
                  , Times.Once);
            service2.Verify(x => x.GetTotalAvailableSize("")
               , Times.Never);

            Assert.Equal(symbol, result.Symbol);
            Assert.Equal(100m, result.MaxSize);
            Assert.Equal(0.1m, result.Spread);
        }
    }
}
