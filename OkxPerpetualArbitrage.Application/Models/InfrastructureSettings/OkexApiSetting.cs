using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Application.Models.InfrastructureSettings
{
    public class OkexApiSetting
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string ApiPassPhrase { get; set; }
        public int ApiMaximumRetries { get; set; }
        public int ApiRetryWaitMiliseconds { get; set; }
    }
}
