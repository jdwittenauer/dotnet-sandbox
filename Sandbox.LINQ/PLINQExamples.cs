using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sandbox.LINQ
{
    /// <summary>
    /// Learning exercise for using PLINQ.
    /// </summary>
    public static class PLINQExamples
    {
        public static void Run()
        {
            Console.WriteLine("PLINQ examples");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            PLinq1();
            PLinq2();
            PLinq3();
            PLinq4();

            Console.WriteLine("Examples complete. Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }

        private static void PLinq1()
        {
            var source = Enumerable.Range(1, 10000);

            // Use AsParallel to opt in to PLINQ
            var result = from num in source.AsParallel()
                           where num % 2 == 0
                           select num;
            Console.WriteLine("Select even numbers in parallel:");
            Console.WriteLine("{0} even numbers out of {1} total", result.Count(), source.Count());

            Console.ReadLine();
        }

        private static void PLinq2()
        {
            var source = Enumerable.Range(1, 10000);

            // Specify the max number of cores to run on
            var result = from num in source.AsParallel().WithDegreeOfParallelism(2)
                           where num % 2 == 0
                           select num;
            Console.WriteLine("Select even numbers in parallel (limit degree):");
            Console.WriteLine("{0} even numbers out of {1} total", result.Count(), source.Count());

            Console.ReadLine();
        }

        private static void PLinq3()
        {
            var source = Enumerable.Range(1, 10000);

            // Specify that the ordering of the sequence is important
            var result = from num in source.AsParallel().AsOrdered()
                           where num % 1000 == 0
                           select num;

            Console.WriteLine("Enumerate numbers in parallel with order retained:");
            foreach (var num in result)
                Console.WriteLine(num);     

            Console.ReadLine();
        }

        private static void PLinq4()
        {
            var source = Enumerable.Range(1, 10000);

            var result = from num in source.AsParallel()
                           where num % 1000 == 0
                           select num;

            // Use for all if results do not need to be merged at the end of the query
            Console.WriteLine("Enumerate numbers in parallel (order not retained):");
            result.ForAll(x => Console.WriteLine(x));

            Console.ReadLine();
        }
    }
}
