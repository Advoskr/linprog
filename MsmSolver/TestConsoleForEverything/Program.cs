﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsmSolver;
using MsmSolver.Misc;
using MsmSolver.Strategies;
using Task = MsmSolver.Task;
using System.Diagnostics;
using MsmSolver.Strategies.Implementations;


namespace TestConsoleForEverything
{
    class Program
    {
        static void Main(string[] args)
        {
            //  var z = new Task();
            // z = new TaskReader().ReadFromSmallFile(new StreamReader("Zadacha 300x300.txt"));
            // SimpleSolver ss = new SimpleSolver(new SingleCoreMathOperationsProvider(), new MTaskBasisFinder());

            //  var answer = ss.SolveTask(z);

            //z = ss.MakeCanonicalForm(z);
            //z = new MTaskBasisFinder().MTask(z);

            //  Basis basis = new CanonicalAdditionalTaskHandler().GetInitialBasis(z);

            //   Console.ReadLine();



            //  for (int i = 0; i < 5; i++)
            // {

            //    var answer = ModularSolverCaller("Zadacha 300x300.txt");
            //  }

            // Console.WriteLine(Jenia.Average());
            // Console.WriteLine("Time:{0}", sw.Elapsed.TotalMilliseconds);
            //Console.WriteLine("Solution: " + answer.Solution);
            //Console.WriteLine("z: " + answer.Z);
            //Console.WriteLine("Kol_vo shagov: " + answer.StepCount);
            // Console.ReadLine();

            // SolveWithSimpleSolver();

            //   SolveWithSimpleSolver();
            //Console.WriteLine(Math.Sign(-0.0014f));

            RunMultipleIterations();

            //GetValue();

            //int counter=10000;
            //for (int i = 0; i < counter; i++)
            //{
            //    var cl = new SomeClass(rectArray);
            //}
            //sw.Stop();
            //Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            //var target = new double[rows][];
            //for (int i = 0; i < rows; i++)
            //{
            //    target[i] = new double[cols];
            //}
            //var target2 = new double[rows][];
            //for (int i = 0; i < rows; i++)
            //{
            //    target2[i] = new double[cols];
            //}

            //var sw = Stopwatch.StartNew();

            //for (int i = 0; i < rows; i++)
            //{
            //    Buffer.BlockCopy(rectArray, doubleSize * i * cols, target[i], 0, doubleSize * cols);
            //}
            //sw.Stop();
            //Console.WriteLine(sw.Elapsed.TotalMilliseconds);

            //sw.Restart();
            //for (int i = 0; i < rows; i++)
            //{
            //    for (int j = 0; j < cols; j++)
            //    {
            //        target2[i][j] = rectArray[i, j];
            //    }
            //}
            //sw.Stop();
            //Console.WriteLine(sw.Elapsed.TotalMilliseconds);

            //sw.Restart();

            //Parallel.For(0, rows, i => Buffer.BlockCopy(rectArray, doubleSize*i*cols, target[i], 0, doubleSize*cols));
            //sw.Stop();
            //Console.WriteLine(sw.Elapsed.TotalMilliseconds);

            //var rectArray2 = new double[rows, cols];
            //for (int i = 0; i < rows; i++)
            //{
            //    Buffer.BlockCopy(target[i],0, rectArray2, doubleSize*i* cols, doubleSize*cols);
            //}

            var str = "X15:Real";
            Console.WriteLine(str.Substring(str.IndexOf(":"), str.Length - 1));

            Console.ReadLine();
        }

        private static void GetValue()
        {
            const int cols = 256;
            const int rows = 256;
            const int doubleSize = sizeof (double);

            double[][] rectArray = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                rectArray[i] = new double[cols];
                for (int j = 0; j < cols; j++)
                {
                    rectArray[i][j] = i + j + 1;
                }
            }

            var matrix = new MsmSolver.Matrix();
            matrix.Initialize(rectArray);
            var sw = Stopwatch.StartNew();
            var mp = new StrassenMathOperationsProvider();
            MsmSolver.Matrix result = null;
            int counter = 10;
            for (int i = 0; i < counter; i++)
            {
                result = mp.Multiply(matrix, matrix);
            }
            Console.WriteLine("Columns:" + result.ColCount);
            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);

            var matrix2 = new Matrix(cols, rows);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix2[i, j] = i + j + 1;
                }
            }

            sw.Restart();
            Matrix res = null;
            for (int i = 0; i < counter; i++)
            {
                res = matrix2*matrix2;
            }
            Console.WriteLine("Columns:" + res.cols);
            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);

            for (int i = 0; i < result.RowCount; i++)
            {
                for (int j = 0; j < result.ColCount; j++)
                {
                    if (res[i, j] != result[i, j])
                        throw new Exception("!");
                }
            }
        }

        private class SomeClass
        {
            public double[,] Values { get; set; }

            public SomeClass(double[,] vals)
            {
                //Values = vals;
                Values = new double[vals.GetLength(0), vals.GetLength(1)];
                var len = vals.GetLength(0)*vals.GetLength(1)*sizeof (double);
                Buffer.BlockCopy(vals, 0, Values, 0, len);
            }
        }

        private static void RunMultipleIterations()
        {
            List<double> Jenia = new List<double>(5);
            int execCount = 2;
            Answer answer = null;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < execCount; i++)
            {
                answer = ModularSolverCaller("Zadacha 300x300.txt");
                Jenia.Add(sw.Elapsed.TotalMilliseconds);
                sw.Stop();
                Console.WriteLine($"Iteration: {i} completed");
                sw.Restart();
            }

            Console.WriteLine("Solution: " + answer.Solution);
            Console.WriteLine("z: " + answer.Z);
            Console.WriteLine("Kol_vo shagov: " + answer.StepCount);

            Console.WriteLine("Execution Time:" + Jenia.Average() + "ms");
        }

        private static void SolveWithSimpleSolver()
        {
            var z = new Task();
            z = new TaskReader().ReadFromSmallFile(new StreamReader("Jenia.txt"));
            var mp = new MulticoreCoreMathOperationsProvider();
            SimpleSolver ss = new SimpleSolver(mp, new MTaskBasisFinder());



            var sw = Stopwatch.StartNew();
            var answer = ss.SolveTask(z);
            sw.Stop();

            Console.WriteLine("Solution: " + answer.Solution);
            Console.WriteLine("z: " + answer.Z);
            Console.WriteLine("Kol_vo shagov: " + answer.StepCount);

            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }

        public static Answer ModularSolverCaller(string taskFile)
        {

            Task z;
            using (var reader = new StreamReader(taskFile))
            {
                z = new TaskReader().ReadFromSmallFile(reader);
            }
            var mathProvider = new StrassenMathOperationsProvider();
            
            var answer = new ModularSolver(mathProvider, new FullParallelDeltasCalculator(mathProvider), new StraightVectorToBasisPutter(mathProvider),
                new FirstIncomingVectorFinder(), new MTaskBasisFinder(), new SimpleOutgoingVectorFinder())
                .SolveTask(z);
            
            return answer;
        }
    }
}