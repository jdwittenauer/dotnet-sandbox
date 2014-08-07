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
    /// Example of the producer/consumer pattern using async tasks.
    /// </summary>
    public static class AsyncProducerConsumer
    {
        public static void Run()
        {
            Console.WriteLine("Async producer/consumer example");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            var buffer = new AsyncProducerConsumerCollection<int>();

            var producer = new Task(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    buffer.Add(i);
                    Console.WriteLine("Producer: {0} added.", i);
                    Thread.Sleep(1000);
                }
            });

            var consumer = new Task(() =>
            {
                // Give the producer time to queue a few items
                Thread.Sleep(4000);

                for (int i = 0; i < 10; i++)
                {
                    var item = buffer.Take();
                    Console.WriteLine("Consumer: {0} removed.", item.Result);
                    Thread.Sleep(500);
                }
            });

            Parallel.Invoke(producer.Start, consumer.Start);
            producer.Wait();
            consumer.Wait();

            Console.WriteLine("");

            Console.WriteLine("Example complete.  Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }
    }

    internal class AsyncProducerConsumerCollection<T>
    {
        private readonly Queue<T> _collection = new Queue<T>();
        private readonly Queue<TaskCompletionSource<T>> _waiting =
            new Queue<TaskCompletionSource<T>>();

        public void Add(T item)
        {
            TaskCompletionSource<T> tcs = null;
            lock (_collection)
            {
                if (_waiting.Count > 0) tcs = _waiting.Dequeue();
                else _collection.Enqueue(item);
            }
            if (tcs != null) tcs.TrySetResult(item);
        }

        public Task<T> Take()
        {
            lock (_collection)
            {
                if (_collection.Count > 0)
                {
                    return Task.FromResult(_collection.Dequeue());
                }
                else
                {
                    var tcs = new TaskCompletionSource<T>();
                    _waiting.Enqueue(tcs);
                    return tcs.Task;
                }
            }
        }
    }
}
