using MediatR;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Features.Joined.Queries
{
    public class GetPositionCloseDataQuery : IRequest<PositionDataDto>
    {
        public string Symbol { get; set; }
    }
}
