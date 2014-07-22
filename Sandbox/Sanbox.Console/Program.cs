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
            TaskExamples.Run();
            ParallelExample.Run();
            ParallelExample2.Run();
            ParallelExample3.Run();
            ConcurrentCollections.Run();
            BlockingCollection.Run();
        }
    }
}
