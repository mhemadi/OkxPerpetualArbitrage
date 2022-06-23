using OkxPerpetualArbitrage.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IPositionOpenCloseChannelWriterLogic
    {
        Task<ApiCommandResponseDto> WriteToChannel(bool open, OpenClosePositionDto openClosePositionDto, CancellationToken cancellationToken);
    }
}
