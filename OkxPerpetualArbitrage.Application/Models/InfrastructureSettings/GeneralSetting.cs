using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Application.Models.InfrastructureSettings
{
    public class GeneralSetting
    {
        public int MaxOpenDemands { get; set; }
        public int MaxCloseDemands { get; set; }
        public int MaxOpenTries { get; set; }
        public int MaxCloseTries { get; set; }
        public int ChuncDollarkValue { get; set; }

        public bool TryToBeMaker { get; set; }
    }
}
