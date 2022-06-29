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
    public class GetPositionOpenDataQueryHandlerTests
    {
        [Fact]
        public async void GetPositionOpenDataCall3Services()
        {
            //  bool checkError = true;
            string symbol = "FIL";
            Mock<IInProgressDemandLogic> service1 = new();
            Mock<IOkxApiLogic> service2 = new();
            Mock<IPotentialPositionProcessorLogic> service3 = new();
            service1.Setup(x => x.GetTotalInProgressRequiredFunds())
                .Returns(Task.FromResult(100m));
            service2.Setup(x => x.GetAvailableUsdtBalance())
                .Returns(Task.FromResult(1000m));
            service3.Setup(x => x.GetPotentialPosition(symbol))
                .Returns(Task.FromResult( new PotentialPosition() { MarkPrice = 10, ContractValuePerp = 1, Spread =0.1m }));
            GetPositionOpenDataQueryHandler handler = new GetPositionOpenDataQueryHandler(service1.Object, service2.Object, service3.Object);

            var result = await handler.Handle(
                 new GetPositionOpenDataQuery() { Symbol = symbol }, CancellationToken.None);

            service1.Verify(x => x.GetTotalInProgressRequiredFunds()
                , Times.Once);
            service2.Verify(x => x.GetAvailableUsdtBalance()
               , Times.Once);
            service3.Verify(x => x.GetPotentialPosition(symbol)
                  , Times.Once);
            service3.Verify(x => x.GetPotentialPosition("")
               , Times.Never);

            Assert.Equal(symbol, result.Symbol);
            Assert.Equal(87m, result.MaxSize);
            Assert.Equal(0.1m, result.Spread);
        }
    }
}
