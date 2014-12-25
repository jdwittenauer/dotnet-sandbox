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
    /// Example that illustrates using tasks to recursively traverse a tree structure.
    /// </summary>
    public static class RecursiveDecomposition
    {
        public static void Run()
        {
            Console.WriteLine("Recursive decomposition example");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            // Build a simple binary tree for the algorithm to traverse
            var tree = new Tree<int>(4,
                new Tree<int>(2,
                    new Tree<int>(1, null, null),
                    new Tree<int>(3, null, null)),
                new Tree<int>(6,
                    new Tree<int>(5, null, null),
                    new Tree<int>(7, null, null)));

            var task = Tree<int>.Walk(tree, x => Console.WriteLine("Value = {0}", x));
            task.Wait();

            Console.WriteLine("");

            Console.WriteLine("Example complete.  Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }
    }

    internal class Tree<T>
    {
        public T Data { get; set; }
        public Tree<T> Left { get; set; }
        public Tree<T> Right { get; set; }

        public Tree(T data, Tree<T> left, Tree<T> right)
        {
            Data = data;
            Left = left;
            Right = right;
        }

        public static Task Walk(Tree<T> root, Action<T> action)
        {
            if (root == null) return _completedTask;
            var t1 = Task.Factory.StartNew(() => action(root.Data));
            var t2 = Task.Factory.StartNew(() => Walk(root.Left, action));
            var t3 = Task.Factory.StartNew(() => Walk(root.Right, action));
            return Task.Factory.ContinueWhenAll(
                new Task[] { t1, t2.Unwrap(), t3.Unwrap() },
                tasks => Task.WaitAll(tasks));
        }

        private static Task _completedTask = ((Func<Task>)(() =>
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }))();
    }
}
