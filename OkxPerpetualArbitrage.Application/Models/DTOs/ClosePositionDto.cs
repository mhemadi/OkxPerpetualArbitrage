using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Models.DTOs
{
    public class ClosePositionDto : OpenClosePositionDto
    {
        public bool IsInstant { get; set; }
    }
}
