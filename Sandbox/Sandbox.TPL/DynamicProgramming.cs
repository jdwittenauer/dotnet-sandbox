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
    /// Dynamic programming example (using an edit distance algorithm) solved with a
    /// recursive parallel implementation using tasks.
    /// </summary>
    public static class DynamicProgramming
    {
        public static void Run()
        {
            Console.WriteLine("Dynamic programming example");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            // TODO

            Console.WriteLine("Example complete.  Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }

        private static void Wavefront(int numRows, int numColumns, Action<int, int> processRowColumnCell)
        {
            // Store the previous row of tasks as well as the previous task in the current row.
            Task[] prevTaskRow = new Task[numColumns];
            Task prevTaskInCurrentRow = null;
            var dependencies = new Task[2];

            // Create a task for each cell
            for (int row = 0; row < numRows; row++)
            {
                prevTaskInCurrentRow = null;
                for (int column = 0; column < numColumns; column++)
                {
                    // In-scope locals for being captured in the task closures
                    int j = row, i = column;
                    // Create a task with the appropriate dependencies
                    Task curTask;

                    if (row == 0 && column == 0)
                    {
                        // Upper-left task kicks everything off, having no dependencies
                        curTask = Task.Factory.StartNew(() => processRowColumnCell(j, i));
                    }
                    else if (row == 0 || column == 0)
                    {
                        // Tasks in the left-most column depend only on the task above them, 
                        // and tasks in the top row depend only on the task to their left
                        var antecedent = column == 0 ? prevTaskRow[0] : prevTaskInCurrentRow;
                        curTask = antecedent.ContinueWith(p =>
                        {
                            p.Wait();
                            processRowColumnCell(j, i);
                        });
                    }
                    else
                    {
                        // All other tasks depend on both the tasks above and to the left
                        dependencies[0] = prevTaskInCurrentRow;
                        dependencies[1] = prevTaskRow[column];
                        curTask = Task.Factory.ContinueWhenAll(dependencies, ps =>
                        {
                            Task.WaitAll(ps);
                            processRowColumnCell(j, i);
                        });
                    }

                    // Keep track of the task just created for future iterations
                    prevTaskRow[column] = prevTaskInCurrentRow = curTask;
                }
            }

            // Wait for the last task to be done
            prevTaskInCurrentRow.Wait();
        }

        private static void Wavefront(int numRows, int numColumns, int numBlocksPerRow, int numBlocksPerColumn,
            Action<int, int, int, int> processBlock)
        {
            // Compute the size of each block
            int rowBlockSize = numRows / numBlocksPerRow;
            int columnBlockSize = numColumns / numBlocksPerColumn;

            Wavefront(numBlocksPerRow, numBlocksPerColumn, (row, column) =>
            {
                int start_i = row * rowBlockSize;
                int end_i = row < numBlocksPerRow - 1 ?
                start_i + rowBlockSize : numRows;
                int start_j = column * columnBlockSize;
                int end_j = column < numBlocksPerColumn - 1 ?
                start_j + columnBlockSize : numColumns;

                processBlock(start_i, end_i, start_j, end_j);
            });
        }

        private static int ParallelEditDistance(string s1, string s2)
        {
            int[,] dist = new int[s1.Length + 1, s2.Length + 1];
            for (int i = 0; i <= s1.Length; i++) dist[i, 0] = i;
            for (int j = 0; j <= s2.Length; j++) dist[0, j] = j;
            int numBlocks = Environment.ProcessorCount * 2;

            Wavefront(s1.Length, s2.Length, numBlocks, numBlocks,
                (start_i, end_i, start_j, end_j) =>
            {
                for (int i = start_i + 1; i <= end_i; i++)
                {
                    for (int j = start_j + 1; j <= end_j; j++)
                    {
                        dist[i, j] = (s1[i - 1] == s2[j - 1]) ?
                            dist[i - 1, j - 1] :
                            1 + Math.Min(dist[i - 1, j],
                            Math.Min(dist[i, j - 1],
                            dist[i - 1, j - 1]));
                    }
                }
            });

            return dist[s1.Length, s2.Length];
        }
    }
}
