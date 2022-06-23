using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Models.DTOs
{
    public class OpenClosePositionDto
    {
        public string Symbol { get; set; }
        public decimal Size { get; set; }
        public decimal MinSpread { get; set; }
    }
}
