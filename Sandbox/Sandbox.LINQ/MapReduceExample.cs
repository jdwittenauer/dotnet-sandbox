using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.LINQ
{
    /// <summary>
    /// Example that demonstrates an implementation of the MapReducee pattern using LINQ operators.
    /// </summary>
    public static class MapReduceExample
    {
        public static void Run()
        {
            Console.WriteLine("MapReduce example using LINQ");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            var files = Directory.EnumerateFiles("*.txt").AsParallel();
            var counts = files.MapReduce(
                path => File.ReadLines(path).SelectMany(line => line.Split(delimiters)),
                word => word,
                group => new[] { new KeyValuePair<string, int>(group.Key, group.Count()) });

            Console.WriteLine("MapReduce complete.");

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
