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
    /// Learning exercies for the task class.  Tasks are units of work that can be composed,
    /// chained, arranged in hierarchies, and used for all sorts of concurrency patterns.
    /// </summary>
    public static class TaskExamples
    {
        public static void Run()
        {
            Console.WriteLine("Task class examples");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            Section1();
            Section2();
            Section3();
            Section4();

            Console.WriteLine("");
        }

        private static void Section1()
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
        }

        private static void Section2()
        {
            // Task with a return type
            Task<int> t3 = new Task<int>(n => Sum((int)n), 1000);
            t3.Start();
            t3.Wait();

            Console.WriteLine("The sum is: " + t3.Result);
            Console.WriteLine("Section 2 complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void Section3()
        {
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
        }

        private static void Section4()
        {
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
