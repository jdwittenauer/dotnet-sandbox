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
    /// Exercise that uses parallel computation to speed up matrix calculations.
    /// </summary>
    public static class ParallelExample
    {
        public static void Run()
        {
            Console.WriteLine("Example using Parallel.For");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            // Set up a comparison of sequential vs. parallel for matrix multiplication
            int colCount = 180;
            int rowCount = 2000;
            int colCount2 = 270;
            double[,] m1 = InitializeMatrix(rowCount, colCount);
            double[,] m2 = InitializeMatrix(colCount, colCount2);
            double[,] result = new double[rowCount, colCount2];

            // First do the sequential version
            Console.WriteLine("Executing sequential loop...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            MultiplyMatricesSequential(m1, m2, result);
            stopwatch.Stop();
            Console.WriteLine("Sequential loop time in milliseconds: {0}", stopwatch.ElapsedMilliseconds);

            // Reset timer and results matrix
            stopwatch.Reset();
            result = new double[rowCount, colCount2];

            // Do the parallel loop
            Console.WriteLine("Executing parallel loop...");
            stopwatch.Start();
            MultiplyMatricesParallel(m1, m2, result);
            stopwatch.Stop();
            Console.WriteLine("Parallel loop time in milliseconds: {0}", stopwatch.ElapsedMilliseconds);

            Console.WriteLine("Example complete.  Press a key to proceed.");
            Console.ReadKey();

            Console.WriteLine("");
        }

        private static double[,] InitializeMatrix(int rows, int cols)
        {
            double[,] matrix = new double[rows, cols];

            Random r = new Random();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = r.Next(100);
                }
            }
            return matrix;
        }

        private static void MultiplyMatricesSequential(double[,] matA, double[,] matB, double[,] result)
        {
            int matACols = matA.GetLength(1);
            int matBCols = matB.GetLength(1);
            int matARows = matA.GetLength(0);

            for (int i = 0; i < matARows; i++)
            {
                for (int j = 0; j < matBCols; j++)
                {
                    for (int k = 0; k < matACols; k++)
                    {
                        result[i, j] += matA[i, k] * matB[k, j];
                    }
                }
            }
        }

        private static void MultiplyMatricesParallel(double[,] matA, double[,] matB, double[,] result)
        {
            int matACols = matA.GetLength(1);
            int matBCols = matB.GetLength(1);
            int matARows = matA.GetLength(0);

            // Parallelize the outer loop to partition the source array by rows
            Parallel.For(0, matARows, i =>
            {
                for (int j = 0; j < matBCols; j++)
                {
                    // Use a temp variable to improve parallel performance
                    double temp = 0;
                    for (int k = 0; k < matACols; k++)
                    {
                        temp += matA[i, k] * matB[k, j];
                    }
                    result[i, j] = temp;
                }
            });
        }
    }
}
