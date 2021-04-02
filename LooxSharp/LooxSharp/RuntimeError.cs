using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooxSharp
{
    public class RuntimeError : Exception
    {
        public readonly Token token;

        public RuntimeError(Token token, string msg) : base(msg)
        {
            this.token = token;
        }

    }
}
