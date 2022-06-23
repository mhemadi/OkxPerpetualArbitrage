using MediatR;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Features.Joined.Queries
{
    public class GetCurrentPositionsQuery : IRequest<List<CurrentPositionListItemDto>>
    {
        public bool CheckError { get; set; } = false;
    }
}
