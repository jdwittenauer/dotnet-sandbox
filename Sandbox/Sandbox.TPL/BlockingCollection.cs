using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sandbox.TPL
{
    /// <summary>
    /// Demonstration of the blocking collection class.  A blocking collection can be
    /// used for producer/consumer patterns, continuation chaining, piplines etc.
    /// </summary>
    public static class BlockingCollection
    {
        public static void Run()
        {
            Console.WriteLine("Blocking collection example with pipelines");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            CancellationTokenSource cts = new CancellationTokenSource();

            // Start up a UI thread for cancellation
            Task.Run(() =>
            {
                if (Console.ReadKey().KeyChar == 'c')
                    cts.Cancel();
            });

            // Generate some source data
            BlockingCollection<int>[] sourceArrays = new BlockingCollection<int>[5];
            for (int i = 0; i < sourceArrays.Length; i++)
                sourceArrays[i] = new BlockingCollection<int>(500);

            Parallel.For(0, sourceArrays.Length * 500, (j) =>
            {
                int k = BlockingCollection<int>.TryAddToAny(sourceArrays, j);
                if (k >= 0)
                    Console.WriteLine("added {0} to source data", j);
            });

            foreach (var arr in sourceArrays)
                arr.CompleteAdding();

            // First filter accepts the ints, keeps back a small percentage 
            // as a processing fee, and converts the results to decimals
            var filter1 = new PipelineFilter<int, decimal>
            (
                sourceArrays,
                (n) => Convert.ToDecimal(n * 0.97),
                cts.Token,
                "filter1"
             );

            // Second filter accepts the decimals and converts them to strings
            var filter2 = new PipelineFilter<decimal, string>
            (
                filter1.Output,
                (s) => String.Format("{0}", s),
                cts.Token,
                "filter2"
             );

            // Third filter uses the constructor with an Action<T> that renders its output 
            // to the screen, not a blocking collection 
            var filter3 = new PipelineFilter<string, string>
            (
                filter2.Output,
                (s) => Console.WriteLine("The final result is {0}", s),
                cts.Token,
                "filter3"
             );

            // Start up the pipeline
            try
            {
                Parallel.Invoke(
                             () => filter1.Run(),
                             () => filter2.Run(),
                             () => filter3.Run()
                         );
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                    Console.WriteLine(ex.Message + ex.StackTrace);
            }

            Console.WriteLine("Example complete.  Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }
    }

    internal class PipelineFilter<TInput, TOutput>
    {
        private Func<TInput, TOutput> _processor = null;
        private Action<TInput> _outputProcessor = null;
        private CancellationToken _token;

        public BlockingCollection<TInput>[] Input;
        public BlockingCollection<TOutput>[] Output = null;

        public string Name { get; private set; }

        public PipelineFilter(BlockingCollection<TInput>[] input, Func<TInput, TOutput> processor,
            CancellationToken token, string name)
        {
            Input = input;
            Output = new BlockingCollection<TOutput>[5];
            for (int i = 0; i < Output.Length; i++)
                Output[i] = new BlockingCollection<TOutput>(500);

            _processor = processor;
            _token = token;
            Name = name;
        }

        // Use this constructor for the final endpoint, which does something like 
        // write to file or screen, instead of pushing to another pipeline filter
        public PipelineFilter(BlockingCollection<TInput>[] input, Action<TInput> renderer,
            CancellationToken token, string name)
        {
            _outputProcessor = renderer;
            _token = token;
            Input = input;
            Name = name;
        }

        public void Run()
        {
            Console.WriteLine("{0} is running", this.Name);
            while (!Input.All(bc => bc.IsCompleted) && !_token.IsCancellationRequested)
            {
                TInput receivedItem;
                int i = BlockingCollection<TInput>.TryTakeFromAny(Input, out receivedItem, 50, _token);

                if (i >= 0)
                {
                    if (Output != null) // we pass data to another blocking collection
                    {
                        TOutput outputItem = _processor(receivedItem);
                        BlockingCollection<TOutput>.AddToAny(Output, outputItem);
                        Console.WriteLine("{0} sent {1} to next", this.Name, outputItem);
                    }
                    else // we're an endpoint
                    {
                        _outputProcessor(receivedItem);
                    }
                }
                else
                    Console.WriteLine("Unable to retrieve data from previous filter");
            }
            if (Output != null)
            {
                foreach (var bc in Output) bc.CompleteAdding();
            }
        }
    }
}
