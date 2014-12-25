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
    /// This example implements a simple asyncronous cache using tasks and lazy initialization.
    /// </summary>
    public static class AsyncCacheExample
    {
        public static void Run()
        {
            Console.WriteLine("Async cache example");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            var cache = new AsyncCache<string, int>(IntegerFactory);

            Console.WriteLine("Attempting a get/add from the cache...");
            var task = cache.GetValue("42");
            task.Start();
            task.Wait();
            Console.WriteLine("Value = {0}", task.Result);

            Console.WriteLine("");

            Console.WriteLine("Example complete.  Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }

        private static Task<int> IntegerFactory(string s)
        {
            return new Task<int>(() => int.Parse(s));
        }
    }

    internal class AsyncCache<TKey, TValue>
    {
        private readonly Func<TKey, Task<TValue>> _valueFactory;
        private readonly ConcurrentDictionary<TKey, Lazy<Task<TValue>>> _map;

        public AsyncCache(Func<TKey, Task<TValue>> valueFactory)
        {
            if (valueFactory == null) throw new ArgumentNullException("loader");
            _valueFactory = valueFactory;
            _map = new ConcurrentDictionary<TKey, Lazy<Task<TValue>>>();
        }

        public Task<TValue> GetValue(TKey key)
        {
            if (key == null) throw new ArgumentNullException("key");
            return _map.GetOrAdd(key, k => new Lazy<Task<TValue>>(() => _valueFactory(k))).Value;
        }
    }
}
