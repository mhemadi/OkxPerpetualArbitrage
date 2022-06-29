using OkxPerpetualArbitrage.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface ICancelDemandLogic
    {
        Task<ApiCommandResponseDto> Cancel(int demandId, CancellationToken cancellationToken);
    }
}
