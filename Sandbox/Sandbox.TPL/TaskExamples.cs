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
            Section5();
            Section6();

            Console.WriteLine("Example complete. Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }

        private static void Section1()
        {
            // Basic usage of tasks
            Task t = new Task(new Action(PrintMessage));
            Task t2 = new Task(() => PrintMessage());

            t.Start();
            t2.Start();
            t.Wait();
            t2.Wait();

            Console.WriteLine("Section 1 complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void Section2()
        {
            // Task with a return type
            Task<int> t = new Task<int>(n => Sum((int)n), 1000);
            t.Start();
            t.Wait();

            Console.WriteLine("The sum is: " + t.Result);
            Console.WriteLine("Section 2 complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void Section3()
        {
            // Using a cancellation token
            CancellationTokenSource cts = new CancellationTokenSource();
            Task<int> t = new Task<int>(() => Sum(1000, cts.Token), cts.Token);
            t.Start();
            cts.Cancel();

            try
            {
                // If the task got canceled, result will throw an exception
                Console.WriteLine("The sum is: " + t.Result);
            }
            catch (AggregateException e)
            {
                e.Handle(x => x is OperationCanceledException);
                Console.WriteLine("Sum was canceled");
            }

            Console.WriteLine("Section 3 complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void Section4()
        {
            // Using a continuation
            Task<int> t = new Task<int>(n => Sum((int)n), 1000);

            Task cwt = t.ContinueWith((task) =>
            {
                Console.WriteLine("The sum is: " + task.Result);
            });

            t.Start();
            cwt.Wait();

            // Continuation with multiple antecedents
            Task<int>[] tasks = new Task<int>[2];
            tasks[0] = new Task<int>(() =>
            {
                return 34;
            });

            tasks[1] = new Task<int>(() =>
            {
                return 8;
            });

            var continuation = Task.Factory.ContinueWhenAll(
                            tasks,
                            (antecedents) =>
                            {
                                int answer = antecedents[0].Result + antecedents[1].Result;
                                Console.WriteLine("The answer is {0}", answer);
                            });

            tasks[0].Start();
            tasks[1].Start();
            continuation.Wait();

            Console.WriteLine("Section 4 complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void Section5()
        {
            // Detached parent/child tasks
            var parent = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Outer task starting.");

                var child = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Nested task starting.");
                    Thread.SpinWait(500000);
                    Console.WriteLine("Nested task complete.");
                });
            });

            parent.Wait();
            Console.WriteLine("Outer task complete.");

            // Detached parent/child tasks with a return value
            var outer = Task<int>.Factory.StartNew(() =>
            {
                Console.WriteLine("Outer task executing.");

                var nested = Task<int>.Factory.StartNew(() =>
                {
                    Console.WriteLine("Nested task starting.");
                    Thread.SpinWait(5000000);
                    Console.WriteLine("Nested task completing.");
                    return 42;
                });

                // Parent will wait for this detached child
                return nested.Result;
            });

            Console.WriteLine("Outer has returned {0}.", outer.Result);

            // Attached parent/child tasks
            var parentWithAttached = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Parent task executing.");
                var child = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Attached child starting.");
                    Thread.SpinWait(5000000);
                    Console.WriteLine("Attached child completing.");
                }, TaskCreationOptions.AttachedToParent);
            });
            parentWithAttached.Wait();
            Console.WriteLine("Parent has completed.");

            Console.WriteLine("Section 5 complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void Section6()
        {
            // Parallel invoke
            Task t = new Task(() => PrintMessage(1));
            Task t2 = new Task(() => PrintMessage(2));
            Task t3 = new Task(() => PrintMessage(3));

            Parallel.Invoke(t.Start, t2.Start, t3.Start);
            t.Wait();
            t2.Wait();
            t3.Wait();

            Console.WriteLine("Section 6 complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void PrintMessage()
        {
            Console.WriteLine("Hello, world!");
        }

        private static void PrintMessage(int task)
        {
            Console.WriteLine("Hello from task {0}!", task);
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
