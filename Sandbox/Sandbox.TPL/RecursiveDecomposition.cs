using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.TPL
{
    /// <summary>
    /// Example that illustrates using tasks to recursively traverse a tree structure.
    /// </summary>
    public static class RecursiveDecomposition
    {
        public static void Run()
        {
            // TODO
        }
    }

    internal class Tree<T>
    {
        private static Task _completedTask = ((Func<Task>)(() =>
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }))();

        public T Data;
        public Tree<T> Left, Right;

        public static Task Walk<T>(Tree<T> root, Action<T> action)
        {
            if (root == null) return _completedTask;
            var t1 = Task.Factory.StartNew(() => action(root.Data));
            var t2 = Task.Factory.StartNew(() => Walk(root.Left, action));
            var t3 = Task.Factory.StartNew(() => Walk(root.Right, action));
            return Task.Factory.ContinueWhenAll(
                new Task[] { t1, t2.Unwrap(), t3.Unwrap() },
                tasks => Task.WaitAll(tasks));
        }
    }
}
