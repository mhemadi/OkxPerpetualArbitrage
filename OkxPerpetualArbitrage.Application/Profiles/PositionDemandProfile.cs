using AutoMapper;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using OkxPerpetualArbitrage.Domain.Entities;

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
