using AutoMapper;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using OkxPerpetualArbitrage.Domain.Entities;

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
