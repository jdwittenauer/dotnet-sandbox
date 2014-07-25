using Sandbox.Dataflow;
using Sandbox.LINQ;
using Sandbox.TPL;

namespace Sanbox.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // LINQ
            LINQExamples.Run();
            MapReduceExample.Run();
            PLINQExamples.Run();

            // TPL
            AsyncCacheExample.Run();
            AsyncExamples.Run();
            AsyncProducerConsumer.Run();
            BlockingCollection.Run();
            ConcurrentCollections.Run();
            DynamicProgramming.Run();
            ParallelExample.Run();
            ParallelExample2.Run();
            ParallelExample3.Run();
            RecursiveDecomposition.Run();
            TaskExamples.Run();

            // Dataflow
            DataflowExamples.Run();
        }
    }
}
