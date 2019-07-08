using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serana.Engine.Exceptions
{
    public class NoOverflowDataException : Exception
    {
        public NoOverflowDataException() 
            : base("Trying to get overflow data but there is no overflow")
        { }
    }
}
