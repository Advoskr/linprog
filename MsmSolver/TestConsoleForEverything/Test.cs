using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestConsoleForEverything
{
    public class MException : Exception
    {
        public MException(string Message)
            : base(Message)
        { }
    }
}
