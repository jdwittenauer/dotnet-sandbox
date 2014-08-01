using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.TPL
{
    /// <summary>
    /// Exercise that performs parallel computation with partial aggregations
    /// using thread-local variables.
    /// </summary>
    public static class ParallelExample3
    {
        public static void Run()
        {
            Console.WriteLine("Parallel example with partial aggregations");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            const int steps = 100000000;

            // TODO

            Console.WriteLine("Example complete.  Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }

        private static double SerialPi(int steps)
        {
            double sum = 0.0;
            double step = 1.0 / (double)steps;

            for (int i = 0; i < steps; i++)
            {
                double x = (i + 0.5) * step;
                double partial = 4.0 / (1.0 + x * x);
                sum += partial;
            }

            return step * sum;
        }

        private static double NaiveParallelPi(int steps)
        {
            double sum = 0.0;
            double step = 1.0 / (double)steps;
            object obj = new object();

            Parallel.For(0, steps, i =>
            {
                double x = (i + 0.5) * step;
                double partial = 4.0 / (1.0 + x * x);
                lock (obj) sum += partial;
            });

            return step * sum;
        }

        private static double ParallelPi(int steps)
        {
            double sum = 0.0;
            double step = 1.0 / (double)steps;
            object obj = new object();

            Parallel.For(0, steps,
                () => 0.0,
                (i, state, partial) =>
                {
                    double x = (i + 0.5) * step;
                    return partial + 4.0 / (1.0 + x * x);
                },
            partial => { lock (obj) sum += partial; });

            return step * sum;
        }
    }
}
