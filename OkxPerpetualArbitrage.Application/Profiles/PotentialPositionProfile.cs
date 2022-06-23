using AutoMapper;
using OkxPerpetualArbitrage.Application.Features.Joined.Queries;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Application.Profiles
{
    public class PotentialPositionProfile : Profile
    {
        public PotentialPositionProfile()
        {
            CreateMap<PotentialPosition, PotentialPositionListItemDto>();
        }
    }
}
