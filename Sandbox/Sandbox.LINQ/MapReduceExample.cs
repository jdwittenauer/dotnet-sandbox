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
    /// Example that demonstrates an implementation of the MapReducee pattern using LINQ operators.
    /// </summary>
    public static class MapReduceExample
    {
        public static void Run()
        {
            Console.WriteLine("MapReduce example");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            var lines = new List<string>();
            lines.Add("This is a simple test for the map reduce program.");
            lines.Add("We will use some generic text for it to parse and get a list of word counts.");

            var counts = lines.MapReduce(
                line => line.Split(delimiters),
                word => word,
                group => new[] { new KeyValuePair<string, int>(group.Key, group.Count()) });

            foreach (var count in counts)
            {
                if (count.Key.Length < 2)
                    Console.WriteLine("Word: {0}\t\t Count: {1}", count.Key, count.Value);
                else
                    Console.WriteLine("Word: {0}\t Count: {1}", count.Key, count.Value);
            }

            Console.WriteLine("");

            Console.WriteLine("Example complete. Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }

        private static char[] delimiters =
            Enumerable.Range(0, 256).Select(i => (char)i)
            .Where(c => Char.IsWhiteSpace(c) || Char.IsPunctuation(c))
            .ToArray();

        private static IEnumerable<TResult> MapReduce<TSource, TMapped, TKey, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TMapped>> map,
            Func<TMapped, TKey> keySelector,
            Func<IGrouping<TKey, TMapped>, IEnumerable<TResult>> reduce)
        {
            return source.SelectMany(map)
            .GroupBy(keySelector)
            .SelectMany(reduce);
        }

        private static ParallelQuery<TResult> MapReduce<TSource, TMapped, TKey, TResult>(
            this ParallelQuery<TSource> source,
            Func<TSource, IEnumerable<TMapped>> map,
            Func<TMapped, TKey> keySelector,
            Func<IGrouping<TKey, TMapped>, IEnumerable<TResult>> reduce)
        {
            return source.SelectMany(map)
            .GroupBy(keySelector)
            .SelectMany(reduce);
        }
    }
}
