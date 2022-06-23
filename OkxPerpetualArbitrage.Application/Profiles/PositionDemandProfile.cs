using AutoMapper;
using OkxPerpetualArbitrage.Application.Features.PositionDemands.Queries;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Application.Profiles
{
    public class PositionDemandProfile : Profile
    {
        public PositionDemandProfile()
        {
            CreateMap<PositionDemand, PositionDemandStatusDto>();
        }
    }
}
