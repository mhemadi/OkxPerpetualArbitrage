﻿using MediatR;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Features.Joined.Queries
{
    public class GetPositionOpenDataQuery : IRequest<PositionDataDto>
    {
        public string Symbol { get; set; }
    }
}
