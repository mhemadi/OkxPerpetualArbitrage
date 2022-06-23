using MediatR;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Features.PositionDemands.Queries
{
    public class GetPositionDemandStatusQuery : IRequest<PositionDemandStatusDto>
    {
        public string Symbol { get; set; }
    }
}
