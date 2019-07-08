using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serana.Engine.Exceptions
{
    public class BadPeHeaderException : Exception
    {
        public BadPeHeaderException()
            : base("Bad PE Header Exception")
        { }
    }
}
