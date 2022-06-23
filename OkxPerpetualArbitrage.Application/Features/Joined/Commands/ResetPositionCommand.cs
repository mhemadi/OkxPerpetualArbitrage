using MediatR;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Features.Joined.Commands
{
    public class ResetPositionCommand : IRequest<ApiCommandResponseDto>
    {
        public string Symbol { get; set; }
    }
}
