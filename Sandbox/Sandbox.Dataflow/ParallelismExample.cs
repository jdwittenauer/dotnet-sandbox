using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Sandbox.Dataflow
{
    /// <summary>
    /// Demonstrates setting the degree of parallelism for a Dataflow block.
    /// </summary>
    public static class ParallelismExample
    {
        public static void Run()
        {
            Console.WriteLine("Dataflow block examples");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            int processorCount = Environment.ProcessorCount;
            int messageCount = processorCount;

            Console.WriteLine("Processor count = {0}.", processorCount);

            TimeSpan elapsed;

            elapsed = TimeDataflowComputations(1, messageCount);
            Console.WriteLine("Degree of parallelism = {0}; message count = {1}; " +
               "elapsed time = {2}ms.", 1, messageCount, (int)elapsed.TotalMilliseconds);

            elapsed = TimeDataflowComputations(processorCount, messageCount);
            Console.WriteLine("Degree of parallelism = {0}; message count = {1}; " +
               "elapsed time = {2}ms.", processorCount, messageCount, (int)elapsed.TotalMilliseconds);

            Console.WriteLine("");

            Console.WriteLine("Example complete. Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }

        private static TimeSpan TimeDataflowComputations(int maxDegreeOfParallelism, int messageCount)
        {
            var workerBlock = new ActionBlock<int>(
               millisecondsTimeout => Thread.Sleep(millisecondsTimeout),
               new ExecutionDataflowBlockOptions
               {
                   MaxDegreeOfParallelism = maxDegreeOfParallelism
               });

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < messageCount; i++)
            {
                workerBlock.Post(1000);
            }

            workerBlock.Complete();
            workerBlock.Completion.Wait();

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }
}
