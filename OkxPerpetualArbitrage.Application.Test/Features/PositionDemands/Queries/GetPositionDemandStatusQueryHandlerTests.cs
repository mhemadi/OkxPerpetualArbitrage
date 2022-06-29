using AutoMapper;
using Moq;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Exceptions;
using OkxPerpetualArbitrage.Application.Features.PositionDemands.Queries;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OkxPerpetualArbitrage.Application.Test.Features.PositionDemands.Queries
{
    public class GetPositionDemandStatusQueryHandlerTests
    {
        [Fact]
        public async void GetPositionDemandStatusQueryCallsGetInProggressDemand()
        {
            string symbol = "FIL";
            Mock<IGetInProgressDemandsLogic> service1 = new();
            Mock<IMapper> service2 = new();
          //  service1.Setup(x => x.GetInProggressDemand(""))
          //      .Returns(Task.FromResult((PositionDemand)(null)));
            service1.Setup(x => x.GetInProggressDemand(symbol))
               .Returns(Task.FromResult(new PositionDemand() { PositionDemandId = 1 }));
            service2.Setup(x => x.Map<PositionDemandStatusDto>(It.IsAny<PositionDemand>()))
                .Returns(new PositionDemandStatusDto() { PositionDemandId = 1 });
            GetPositionDemandStatusQueryHandler handler = new GetPositionDemandStatusQueryHandler(service1.Object, service2.Object);

           var result = await handler.Handle(
                new GetPositionDemandStatusQuery() { Symbol = symbol }, CancellationToken.None);

            service1.Verify(x => x.GetInProggressDemand(symbol)
                , Times.Once);
            service1.Verify(x => x.GetInProggressDemand("")
                , Times.Never);

          await  Assert.ThrowsAsync<OkxPerpetualArbitrageCustomException>(() => handler.Handle(
                new GetPositionDemandStatusQuery() { Symbol = "" }, CancellationToken.None));

            Assert.Equal(1, result.PositionDemandId);
        }
    }
}
