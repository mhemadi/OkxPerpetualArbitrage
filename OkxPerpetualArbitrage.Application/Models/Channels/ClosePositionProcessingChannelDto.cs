using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Application.Models.Channels
{
    public class ClosePositionProcessingChannelDto : OpenClosePositionProcessingChannelDto
    {
        public bool IsInstant { get; set; }
    }
}
