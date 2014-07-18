using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Async;
using Sandbox.LINQ;
using Sandbox.TPL;

namespace Sanbox.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Async.Run();
            LINQ.Run();
            TPL.Run();
        }
    }
}
