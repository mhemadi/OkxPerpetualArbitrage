using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Exceptions
{
    public class OkxPerpetualArbitrageCustomException : Exception
    {
        public OkxPerpetualArbitrageCustomException()
        {
        }

        public OkxPerpetualArbitrageCustomException(string message)
            : base(message)
        {
        }

        public OkxPerpetualArbitrageCustomException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}