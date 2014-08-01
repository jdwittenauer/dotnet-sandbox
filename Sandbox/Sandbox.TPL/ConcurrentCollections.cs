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
    /// Demonstration of the four concurrent collection types.  Concurrent collections are 
    /// thread-safe versions of the equivalent generic collections that are better suited for 
    /// multi-threaded environments.
    /// </summary>
    public static class ConcurrentCollections
    {
        public static void Run()
        {
            Console.WriteLine("Conncurrent collections examples");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            ConcurrentDictionaryExample();
            ConcurrentQueueExample();
            ConcurrentStackExample();
            ConcurrentBagExample();

            Console.WriteLine("Example complete.  Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }

        private static void ConcurrentDictionaryExample()
        {
            // Concurrent dictionary example
            int NUMITEMS = 64;
            int initialCapacity = 101;

            int cores = Environment.ProcessorCount;
            int concurrencyLevel = cores * 2;

            // Construct the dictionary with the desired concurrency level and initial capacity
            ConcurrentDictionary<int, int> cd = new ConcurrentDictionary<int, int>(concurrencyLevel, initialCapacity);

            // Initialize the dictionary 
            for (int i = 0; i < NUMITEMS; i++) cd[i] = i * i;

            Console.WriteLine("The square of 23 is {0} (should be {1})", cd[23], 23 * 23);

            Console.WriteLine("Concurrent dictionary complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void ConcurrentQueueExample()
        {
            // Concurrent queue example
            ConcurrentQueue<int> cq = new ConcurrentQueue<int>();

            // Populate the queue
            for (int i = 0; i < 10000; i++) cq.Enqueue(i);

            // Peek at the first element
            int result;
            if (!cq.TryPeek(out result))
            {
                Console.WriteLine("TryPeek failed when it should have succeeded");
            }
            else if (result != 0)
            {
                Console.WriteLine("Expected TryPeek result of 0, got {0}", result);
            }

            // An action to consume the queue
            int outerSum = 0;
            Action action = () =>
            {
                int localSum = 0;
                int localValue;
                while (cq.TryDequeue(out localValue)) localSum += localValue;
                Interlocked.Add(ref outerSum, localSum);
            };

            // Start 4 concurrent consuming actions
            Parallel.Invoke(action, action, action, action);

            Console.WriteLine("outerSum = {0}, should be 49995000", outerSum);

            Console.WriteLine("Concurrent queue complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void ConcurrentStackExample()
        {
            // Concurrent stack example
            int errorCount = 0;
            ConcurrentStack<int> cs = new ConcurrentStack<int>();

            // Push some consecutively numbered ranges
            cs.PushRange(new int[] { 1, 2, 3, 4, 5, 6, 7 });
            cs.PushRange(new int[] { 8, 9, 10 });
            cs.PushRange(new int[] { 11, 12, 13, 14 });
            cs.PushRange(new int[] { 15, 16, 17, 18, 19, 20 });
            cs.PushRange(new int[] { 21, 22 });
            cs.PushRange(new int[] { 23, 24, 25, 26, 27, 28, 29, 30 });

            // Now read them back, 3 at a time, concurrently
            Parallel.For(0, 10, i =>
            {
                int[] range = new int[3];
                if (cs.TryPopRange(range) != 3)
                {
                    Console.WriteLine("TryPopRange failed unexpectedly");
                    Interlocked.Increment(ref errorCount);
                }

                // Each range should be consecutive integers and reversed from the original order
                if (!range.Skip(1).SequenceEqual(range.Take(range.Length - 1).Select(x => x - 1)))
                {
                    Console.WriteLine("Expected consecutive ranges.  range[0]={0}, range[1]={1}", range[0], range[1]);
                    Interlocked.Increment(ref errorCount);
                }
            });

            // Stack should be empty
            if (!cs.IsEmpty)
            {
                Console.WriteLine("Expected IsEmpty to be true after emptying");
                errorCount++;
            }

            if (errorCount == 0) Console.WriteLine("OK!");

            Console.WriteLine("Concurrent stack complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void ConcurrentBagExample()
        {
            // Concurrent bag example
            ConcurrentBag<int> cb = new ConcurrentBag<int>();
            cb.Add(1);
            cb.Add(2);
            cb.Add(3);

            // Consume the items in the bag 
            int item;
            while (!cb.IsEmpty)
            {
                if (cb.TryTake(out item))
                    Console.WriteLine(item);
                else
                    Console.WriteLine("TryTake failed for non-empty bag");
            }

            // Bag should be empty 
            if (cb.TryPeek(out item))
                Console.WriteLine("TryPeek succeeded for empty bag!");

            Console.WriteLine("Concurrent bag complete.  Press a key to proceed.");
            Console.ReadLine();
        }
    }
}
