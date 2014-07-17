using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TPL
{
    /// <summary>
    /// Learning exercise for parallel processing and concurrency on the .NET framework.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Basic usage of tasks
            Task t1 = new Task(new Action(PrintMessage));
            Task t2 = new Task(() => PrintMessage());

            t1.Start();
            t2.Start();
            t1.Wait();
            t2.Wait();

            Console.WriteLine("Section 1 complete.  Press a key to proceed.");
            Console.ReadLine();


            // Task with a return type
            Task<int> t3 = new Task<int>(n => Sum((int)n), 1000);
            t3.Start();
            t3.Wait();

            Console.WriteLine("The sum is: " + t3.Result);
            Console.WriteLine("Section 2 complete.  Press a key to proceed.");
            Console.ReadLine();


            // Using a cancellation token
            CancellationTokenSource cts = new CancellationTokenSource();
            Task<int> t4 = new Task<int>(() => Sum(1000, cts.Token), cts.Token);
            t4.Start();
            cts.Cancel();

            try
            {
                // If the task got canceled, result will through an exception
                Console.WriteLine("The sum is: " + t4.Result);
            }
            catch (AggregateException ae)
            {
                ae.Handle(e => e is OperationCanceledException);
                Console.WriteLine("Sum was canceled");
            }

            Console.WriteLine("Section 3 complete.  Press a key to proceed.");
            Console.ReadLine();


            // Create task, defer starting it, and continue with another task
            Task<int> t5 = new Task<int>(n => Sum((int)n), 1000);
            t5.Start();

            Task cwt = t5.ContinueWith(task => Console.WriteLine("The sum is: " + task.Result));
            cwt.Wait();

            Console.WriteLine("Section 4 complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void PrintMessage()
        {
            Console.WriteLine("Hello, world!");
        }

        private static int Sum(int n)
        {
            int sum = 0;
            for (; n > 0; n--)
                checked { sum += n; }
            return sum;
        }

        private static int Sum(int n, CancellationToken ct)
        {
            int sum = 0;
            for (; n > 0; n--)
            {
                ct.ThrowIfCancellationRequested();
                checked { sum += n; }
            }
            return sum;
        }
    }
}
