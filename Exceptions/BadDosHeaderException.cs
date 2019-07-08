using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serana.Engine.Exceptions
{
    public class BadDosHeaderException : Exception
    {
        public BadDosHeaderException()
            : base("Bad DOS Header Exception")
        {}
    }
}
