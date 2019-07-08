using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serana.Engine.Exceptions
{
    public class FailToOpenException : Exception
    {
        public FailToOpenException()
            : base("Fail to open the file")
        { }
    }
}
