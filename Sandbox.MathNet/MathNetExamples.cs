using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.Statistics;

namespace Sandbox.MathNet
{
    /// <summary>
    /// Learning exercieses for the Math.NET numerics library.
    /// </summary>
    public static class MathNetExamples
    {
        public static void Run()
        {
            Console.WriteLine("Math.NET examples");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            Section1();
            Section2();
            Section3();

            Console.WriteLine("Example complete. Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }

        private static void Section1()
        {
            // Create a dense matrix with 3 rows and 4 columns
            Matrix<double> m = Matrix<double>.Build.Random(3, 4);

            // Create a dense zero-vector of length 10
            Vector<double> v = Vector<double>.Build.Dense(10);

            var M = Matrix<double>.Build;
            var V = Vector<double>.Build;

            // Build the same as above
            var m2 = M.Random(3, 4);
            var v2 = V.Dense(10);

            // 3x4 dense matrix filled with zeros
            M.Dense(3, 4);

            // 3x4 dense matrix filled with 1.0.
            M.Dense(3, 4, 1.0);

            // 3x4 dense matrix where each field is initialized using a function
            M.Dense(3, 4, (i, j) => 100 * i + j);

            // 3x4 square dense matrix with each diagonal value set to 2.0
            M.DenseDiagonal(3, 4, 2.0);

            // 3x3 dense identity matrix
            M.DenseIdentity(3);

            // 3x4 dense random matrix sampled from a Gamma distribution
            M.Random(3, 4, new Gamma(1.0, 5.0));

            // Copy of an existing matrix (can also be sparse or diagonal)
            Matrix<double> x = m;
            M.DenseOfMatrix(x);

            // From a 2D-array
            double[,] x2 = { { 1.0, 2.0 }, { 3.0, 4.0 } };
            M.DenseOfArray(x2);

            // From an enumerable of values and their coordinates
            Tuple<int, int, double>[] x3 = { Tuple.Create(0, 0, 2.0), Tuple.Create(0, 1, -3.0) };
            M.DenseOfIndexed(3, 4, x3);

            // From an enumerable in column major order (column by column)
            double[] x4 = { 1.0, 2.0, 3.0, 4.0 };
            M.DenseOfColumnMajor(2, 2, x4);

            // From an enumerable of enumerable-columns (optional with explicit size)
            IEnumerable<IEnumerable<double>> x5 = new List<List<double>> 
                { new List<double> { 1.0, 2.0 }, new List<double> { 1.0, 2.0 } };
            M.DenseOfColumns(x5);

            // From a params-array of array-columns (or an enumerable of them)
            M.DenseOfColumnArrays(new[] { 2.0, 3.0 }, new[] { 4.0, 5.0 });

            // From a params-array of column vectors (or an enumerable of them)
            M.DenseOfColumnVectors(V.Random(3), V.Random(3));

            // Equivalent variants also for rows or diagonals:
            M.DenseOfRowArrays(new[] { 2.0, 3.0 }, new[] { 4.0, 5.0 });
            M.DenseOfDiagonalArray(new[] { 2.0, 3.0, 4.0 });

            // Standard-distributed random vector of length 10
            V.Random(10);

            // All-zero vector of length 10
            V.Dense(10);

            // Each field is initialized using a function
            V.Dense(10, i => i * i);
        }

        private static void Section2()
        {
            var M = Matrix<double>.Build;
            var V = Vector<double>.Build;
            var X = M.Random(4, 4, new Gamma(1.0, 5.0));
            var y = M.Random(4, 4, new Gamma(1.0, 5.0));

            // Vector norms
            var v = V.Dense(10);
            v.L1Norm();
            v.L2Norm();
            v.InfinityNorm();
            v.Norm(0);
            v.Normalize(0);

            // Matrix norms
            X.L1Norm();
            X.L2Norm();
            X.InfinityNorm();
            X.FrobeniusNorm();
            X.RowNorms(1);
            X.ColumnNorms(1);
            X.NormalizeRows(1);
            X.NormalizeColumns(1);

            // Sums
            v.Sum();
            v.SumMagnitudes();
            X.RowSums();
            X.ColumnSums();

            // Decompositions
            double[,] array = { { 1.0, 0.0 }, { 0.0, 1.0 } };
            var X2 = M.DenseOfArray(array);
            X2.Cholesky();
            X2.LU();
            X2.QR(QRMethod.Full);
            X2.GramSchmidt();
            X2.Svd(true);
            X2.Evd(Symmetricity.Symmetric);

            // Indexing
            var element = X[0, 0];
            X.Column(0);
            X.Row(0);
            X.SubMatrix(0, 1, 0, 1);

            // Other operations
            X.SetRow(3, V.Random(4));
            X.SetColumn(3, V.Random(4));
            X.Clear();
            X.ClearRow(0);
            X.ClearColumn(0);
            X.ClearSubMatrix(1, 2, 1, 2);
            X.RemoveRow(0);
            X.RemoveColumn(0);
            X.CoerceZero(1e-14);

            // Iterators
            X.Enumerate(Zeros.Include);
            X.EnumerateIndexed();
            X.EnumerateRows();
            X.EnumerateColumns();

            // Map & reduce
            X.Map(f => f * f);
            X.MapInplace(f => f * f);
            X.MapIndexed((i, j, f) => f * f);
            X.MapConvert(f => f, X);
            X.ReduceRows((f, g) => f + g);

            // Linear regression equation
            var res = (X.Transpose() * X).Inverse() * (X.Transpose() * y);
        }

        private static void Section3()
        {
            // Descriptive statistics
            var samples = new ChiSquared(5).Samples().Take(1000);
            var samples2 = new ChiSquared(5).Samples().Take(1000);
            var whiteNoise = Generate.Gaussian(1000, 10.0, 2.0);
            var statistics = new DescriptiveStatistics(samples);

            var largestElement = statistics.Maximum;
            var smallestElement = statistics.Minimum;

            var mean = statistics.Mean;
            var variance = statistics.Variance;
            var stdDev = statistics.StandardDeviation;

            var kurtosis = statistics.Kurtosis;
            var skewness = statistics.Skewness;

            Statistics.Covariance(samples, samples2);

            Statistics.LowerQuartile(whiteNoise);
            Statistics.UpperQuartile(whiteNoise);
            Statistics.FiveNumberSummary(whiteNoise);
            Statistics.Percentile(whiteNoise, 90);
            Statistics.Ranks(whiteNoise);


            // Probability distributions
            var gamma = new Gamma(2.0, 1.5);

            double gmean = gamma.Mean;
            double gvariance = gamma.Variance;
            double gentropy = gamma.Entropy;

            double i = gamma.Density(2.3);
            double j = gamma.DensityLn(2.3);
            double k = gamma.CumulativeDistribution(0.7);

            double randomSample = gamma.Sample();


            // Solving linear equations
            var A = Matrix<double>.Build.DenseOfArray(new double[,] {
                { 3, 2, -1 },
                { 2, -2, 4 },
                { -1, 0.5, -1 }
            });
            var b = Vector<double>.Build.Dense(new double[] { 1, -2, 0 });
            var x = A.Solve(b);
        }
    }
}
