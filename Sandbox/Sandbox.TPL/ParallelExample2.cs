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
    /// Exercise that uses parallel computations to speed up an operation.
    /// </summary>
    public static class ParallelExample2
    {
        public static void Run()
        {
            Console.WriteLine("Example using Parallel.ForEach");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            // Create and initialize an array of random numbers
            Random r = new Random();
            int elements = 100000;
            double[] numbersSerial = new double[elements];
            double[] numbersParallel = new double[elements];
            for (int i = 0; i < elements; i++)
            {
                var temp = r.NextDouble();
                numbersSerial[i] = temp;
                numbersParallel[i] = temp;
            }

            Console.WriteLine("Applying a function to an array of numbers in serial...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < elements; i++)
            {
                numbersSerial[i] = 1 / (1 + Math.Pow(Math.E, numbersSerial[i]));
            }

            stopwatch.Stop();
            Console.WriteLine("Serial loop time in milliseconds: {0}", stopwatch.ElapsedMilliseconds);

            Console.WriteLine("Applying a function to an array of numbers in parallel...");
            stopwatch.Reset();
            stopwatch.Start();

            Parallel.ForEach(numbersParallel, number =>
            {
                // Apply some arbitrary code to the elements of the array and it will parallelize
                // automatically (in this case we're applying the sigmoid function)
                number = 1 / (1 + Math.Pow(Math.E, number));
            });

            stopwatch.Stop();
            Console.WriteLine("Parallel loop time in milliseconds: {0}", stopwatch.ElapsedMilliseconds);

            Console.WriteLine("");

            Console.WriteLine("Example complete.  Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }
    }
}
