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
    /// Pre-defined block type examples from the DataFlow library.
    /// </summary>
    public static class BlockTypeExamples
    {
        public static void Run()
        {
            Console.WriteLine("Dataflow block examples");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            Section1();
            Section2();
            Section3();
            Section4();
            Section5();
            Section6();
            Section7();
            Section8();
            Section9();

            Console.WriteLine("Example complete. Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }

        private static void Section1()
        {
            // Buffer block
            var bufferBlock = new BufferBlock<int>();

            // Post several messages to the block
            for (int i = 0; i < 3; i++)
            {
                bufferBlock.Post(i);
            }

            // Receive the messages back from the block
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(bufferBlock.Receive());
            }

            Console.WriteLine("Section 1 complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void Section2()
        {
            // Broadcast block
            var broadcastBlock = new BroadcastBlock<double>(null);

            // Post a message to the block
            broadcastBlock.Post(Math.PI);

            // Receive the messages back from the block several times.
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(broadcastBlock.Receive());
            }

            Console.WriteLine("Section 2 complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void Section3()
        {
            // Write once block
            var writeOnceBlock = new WriteOnceBlock<string>(null);

            // Post several messages to the block in parallel
            Parallel.Invoke(
               () => writeOnceBlock.Post("Message 1"),
               () => writeOnceBlock.Post("Message 2"),
               () => writeOnceBlock.Post("Message 3"));

            // Only the first recieved message was added
            Console.WriteLine(writeOnceBlock.Receive());

            Console.WriteLine("Section 3 complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void Section4()
        {
            // Action block
            var actionBlock = new ActionBlock<int>(n => Console.WriteLine(n));

            // Post several messages to the block
            for (int i = 0; i < 3; i++)
            {
                actionBlock.Post(i * 10);
            }

            // Set the block to the completed state and wait for all tasks to finish
            actionBlock.Complete();
            actionBlock.Completion.Wait();

            Console.WriteLine("Section 4 complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void Section5()
        {
            // Transform block
            var transformBlock = new TransformBlock<int, double>(n => Math.Sqrt(n));

            // Post several messages to the block
            transformBlock.Post(10);
            transformBlock.Post(20);
            transformBlock.Post(30);

            // Read the output messages from the block
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(transformBlock.Receive());
            }

            Console.WriteLine("Section 5 complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void Section6()
        {
            // Transform many block
            var transformManyBlock = new TransformManyBlock<string, char>(s => s.ToCharArray());

            // Post two messages to the block
            transformManyBlock.Post("Hello");
            transformManyBlock.Post("World");

            // Receive all output values from the block. 
            for (int i = 0; i < ("Hello" + "World").Length; i++)
            {
                Console.WriteLine(transformManyBlock.Receive());
            }

            Console.WriteLine("Section 6 complete.  Press a key to proceed.");
            Console.ReadLine();
        }
        private static void Section7()
        {
            // Batch block
            var batchBlock = new BatchBlock<int>(10);

            // Post several values to the block
            for (int i = 0; i < 13; i++)
            {
                batchBlock.Post(i);
            }

            // Set the block to the completed state
            batchBlock.Complete();

            // Print the sum of both batches
            Console.WriteLine("The sum of the elements in batch 1 is {0}.",
               batchBlock.Receive().Sum());

            Console.WriteLine("The sum of the elements in batch 2 is {0}.",
               batchBlock.Receive().Sum());

            Console.WriteLine("Section 7 complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void Section8()
        {
            // Join block
            var joinBlock = new JoinBlock<int, int, char>();

            // Post two values to each target of the join
            joinBlock.Target1.Post(3);
            joinBlock.Target1.Post(6);

            joinBlock.Target2.Post(5);
            joinBlock.Target2.Post(4);

            joinBlock.Target3.Post('+');
            joinBlock.Target3.Post('-');

            // Receive each group of values and apply the operator part to the number parts
            for (int i = 0; i < 2; i++)
            {
                var data = joinBlock.Receive();
                switch (data.Item3)
                {
                    case '+':
                        Console.WriteLine("{0} + {1} = {2}",
                           data.Item1, data.Item2, data.Item1 + data.Item2);
                        break;
                    case '-':
                        Console.WriteLine("{0} - {1} = {2}",
                           data.Item1, data.Item2, data.Item1 - data.Item2);
                        break;
                    default:
                        Console.WriteLine("Unknown operator '{0}'.", data.Item3);
                        break;
                }
            }

            Console.WriteLine("Section 8 complete.  Press a key to proceed.");
            Console.ReadLine();
        }

        private static void Section9()
        {
            Func<int, int> DoWork = n =>
            {
                if (n < 0)
                    throw new ArgumentOutOfRangeException();
                return n;
            };

            // Batched join block
            var batchedJoinBlock = new BatchedJoinBlock<int, Exception>(7);

            // Post several items to the block
            foreach (int i in new int[] { 5, 6, -7, -22, 13, 55, 0 })
            {
                try
                {
                    // Post the result of the worker to the first target of the block
                    batchedJoinBlock.Target1.Post(DoWork(i));
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // If an error occurred, post the exception to the second target of the block
                    batchedJoinBlock.Target2.Post(e);
                }
            }

            // Read the results from the block
            var results = batchedJoinBlock.Receive();

            // Print the results
            foreach (int n in results.Item1)
            {
                Console.WriteLine(n);
            }

            // Print failures
            foreach (Exception e in results.Item2)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Section 9 complete.  Press a key to proceed.");
            Console.ReadLine();
        }
    }
}
