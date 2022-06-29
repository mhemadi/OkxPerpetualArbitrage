using Moq;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Features.PositionDemands.Commands;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OkxPerpetualArbitrage.Application.Test.Features.PositionDemands.Commands
{
    public class CancelDemandCommandHandlerTests
    {
        [Fact]
        public void CancelDemandCommand1CallsCancel()
        {
            int demandId = 100;
            Mock<ICancelDemandLogic> service = new();
            service.Setup(x => x.Cancel(demandId, CancellationToken.None))
                .Returns(Task.FromResult(new ApiCommandResponseDto(){ }));
            CancelDemandCommandHandler handler = new CancelDemandCommandHandler(service.Object);

            handler.Handle(
                new CancelDemandCommand() { DemandId = demandId }, CancellationToken.None);

            service.Verify(x => x.Cancel(demandId, CancellationToken.None)
                , Times.Once);
            service.Verify(x => x.Cancel(99, CancellationToken.None)
                , Times.Never);

        }
    }
}
