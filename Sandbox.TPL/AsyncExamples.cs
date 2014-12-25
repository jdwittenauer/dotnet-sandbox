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
    /// Asyncronous programming examples.
    /// </summary>
    public static class AsyncExamples
    {
        public static void Run()
        {
            Console.WriteLine("Async programming examples");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            // Calling an async method with no return type 
            var task = PrintAsync();
            task.Wait();

            // Async method that returns a value
            var result = ReturnIntAsync();
            result.Wait();
            Console.WriteLine("Method returned the value {0}", result.Result);

            // Async method with a progress indicator
            var progress = new Progress<int>(p => Console.WriteLine("Task progress: {0}", p));
            var listResult = ReturnListAsync(progress);
            listResult.Wait();
            Console.WriteLine("Task complete!");

            Console.WriteLine("Example complete.  Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }

        private static async Task PrintAsync()
        {
            await Task.Factory.StartNew(() => Console.WriteLine("Hello, world!"));
        }

        private static async Task<int> ReturnIntAsync()
        {
            int result = await Task<int>.Factory.StartNew(() => { return 42; });
            return result;
        }

        private static async Task<List<int>> ReturnListAsync(IProgress<int> progress)
        {
            var result = new List<int>();

            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                result.Add(i);
                progress.Report(i);
            }

            return result;
        }
    }
}
