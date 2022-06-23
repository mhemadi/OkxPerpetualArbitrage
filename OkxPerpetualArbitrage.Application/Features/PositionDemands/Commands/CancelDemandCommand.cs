using MediatR;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Features.PositionDemands.Commands
{
    public class CancelDemandCommand : IRequest<ApiCommandResponseDto>
    {
        public int DemandId { get; set; }
    }
}
