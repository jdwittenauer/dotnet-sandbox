using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.TPL
{
    public static class AsyncProducerConsumer
    {
        public static void Run()
        {
            // TODO
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
