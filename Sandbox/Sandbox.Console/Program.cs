using Sandbox.Dataflow;
using Sandbox.LINQ;
using Sandbox.Lucene;
using Sandbox.MathNet;
using Sandbox.Reactive;
using Sandbox.TPL;

namespace Sandbox.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // LINQ
            LINQExamples.Run();
            LinqXMLExamples.Run();
            PLINQExamples.Run();
            MapReduceExample.Run();

            // TPL
            AsyncExamples.Run();
            AsyncCacheExample.Run();
            AsyncProducerConsumer.Run();
            TaskExamples.Run();
            ParallelExample.Run();
            ParallelExample2.Run();
            ParallelExample3.Run();
            ConcurrentCollections.Run();
            BlockingCollection.Run();
            DynamicProgramming.Run();
            RecursiveDecomposition.Run();

            // Dataflow
            BlockTypeExamples.Run();
            ParallelismExample.Run();
            PipelineExample.Run();

            // Reactive Extenstions
            RxExamples.Run();

            // Math .NET
            MathNetExamples.Run();

            // Lucene .NET
            LuceneExample.Run();
        }
    }
}
