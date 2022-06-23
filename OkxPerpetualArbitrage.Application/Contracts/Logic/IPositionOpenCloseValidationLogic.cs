using OkxPerpetualArbitrage.Application.Features.Joined.Commands;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IPositionOpenCloseValidationLogic
    {
        Task<ApiCommandResponseDto> PostValidateClose(ClosePositionDto closePositionDto, int positionDemandId, PotentialPosition pp);
        Task<ApiCommandResponseDto> PostValidateOpen(OpenPositionDto openPositionDto, int positionDemandId, PotentialPosition pp);
        Task<ApiCommandResponseDto> PreValidateClose(ClosePositionDto closePositionDto, PotentialPosition pp);
        Task<ApiCommandResponseDto> PreValidateOpen(OpenPositionDto openPositionDto, PotentialPosition pp);
    }
}
