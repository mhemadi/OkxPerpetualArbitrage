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
    public class ClosePositionCommandHandlerTests
    {
        [Fact]
        public void ClosePositionCommandWritesToCloseChannel()
        {
            ClosePositionDto closePositionDto = new ClosePositionDto() { MinSpread = 0, Size = 1, Symbol = "FIL" };
            Mock<IPositionOpenCloseChannelWriterLogic> service = new();
            service.Setup(x => x.WriteToChannel(It.IsAny<bool>(), closePositionDto, CancellationToken.None))
                .Returns(Task.FromResult(new ApiCommandResponseDto()));
            ClosePositionCommandHandler closePositionCommandHandler = new ClosePositionCommandHandler(service.Object);
            
            closePositionCommandHandler.Handle(           
                new ClosePositionCommand() { ClosePositionDto = closePositionDto }, CancellationToken.None);

            service.Verify(x => x.WriteToChannel(false, It.IsAny<OpenClosePositionDto>(), CancellationToken.None)
                , Times.Once);
            service.Verify(x => x.WriteToChannel(true, It.IsAny<OpenClosePositionDto>(), CancellationToken.None)
                , Times.Never);

        }
    }
}
