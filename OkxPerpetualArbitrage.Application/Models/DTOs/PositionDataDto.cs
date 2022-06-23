using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Models.DTOs
{
    public class PositionDataDto
    {
        public string Symbol { get; set; }
        public decimal MaxSize { get; set; }
        public decimal Spread { get; set; }
    }
}
