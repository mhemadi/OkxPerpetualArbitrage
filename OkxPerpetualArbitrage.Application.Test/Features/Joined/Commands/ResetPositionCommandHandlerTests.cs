using Moq;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Features.Joined.Commands;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OkxPerpetualArbitrage.Application.Test.Features.Joined.Commands
{
    public class ResetPositionCommandHandlerTests
    {
        [Fact]
        public void ResetPositionCommandCallsResetPosition()
        {
            string symbol = "FIL";
            Mock<IResetPositionLogic> service = new();
            service.Setup(x => x.ResetPosition(symbol, CancellationToken.None))
                .Returns(Task.FromResult(new ApiCommandResponseDto()));
            ResetPositionCommandHandler handler = new ResetPositionCommandHandler(service.Object);

            handler.Handle(
                new ResetPositionCommand() {  Symbol = symbol }, CancellationToken.None);

            service.Verify(x => x.ResetPosition(symbol, CancellationToken.None)
                , Times.Once);
            service.Verify(x => x.ResetPosition("", CancellationToken.None)
                , Times.Never);

        }
    }
}
