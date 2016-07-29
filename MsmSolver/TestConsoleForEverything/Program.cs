using System;
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


namespace TestConsoleForEverything
{
    class Program
    {
     public static List<double> Jenia = new List<double>(5);
        static void Main(string[] args)
        {
            var z = new Task();
            z = new TaskReader().ReadFromSmallFile(new StreamReader("Jenia.txt"));
            SimpleSolver ss = new SimpleSolver(new SingleCoreMathOperationsProvider());

            z = ss.MakeCanonicalForm(z);
            z = new CreationMTask().MTask(z);

            Console.ReadLine();



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



        }

        private static void SolveWithSimpleSolver()
        {
            var z = new Task();
            z = new TaskReader().ReadFromSmallFile(new StreamReader("Zadacha 300x300.txt"));
            var mp = new SingleCoreMathOperationsProvider();
            SimpleSolver ss = new SimpleSolver(mp);


            var answer = ss.SolveTask(z);


            Console.WriteLine("Solution: " + answer.Solution);
            Console.WriteLine("z: " + answer.Z);
            Console.WriteLine("Kol_vo shagov: " + answer.StepCount);
            Console.ReadLine();
        }

        public static Answer ModularSolverCaller(string taskFile)
        {

            Task z;
            using (var reader = new StreamReader(taskFile))
            {
                z = new TaskReader().ReadFromSmallFile(reader);
            }
            var mathProvider = new MulticoreCoreMathOperationsProvider();
            var sw = Stopwatch.StartNew();
            var answer = new ModularSolver(mathProvider, new FullParallelDeltasCalculator(mathProvider), new StraightVectorToBasisPutter(mathProvider), 
                new FirstIncomingVectorFinder(), new CanonicalInitialBasisFinder(), new SimpleOutgoingVectorFinder())
                .SolveTask(z);
            sw.Stop();
            Jenia.Add(sw.Elapsed.TotalMilliseconds);
            return answer;
        }

        public static Matrix MultiplyMatricesParallel(Matrix matA, Matrix matB)
        {
            if (matA.ColCount == matB.RowCount)
            {

               double[][] val = new double[matA.RowCount][];
                for (int i = 0; i < matA.RowCount; i++)
                {
                    val[i] = new double[matB.ColCount];
               }
                int matACols = matA.ColCount;
                int matBCols = matB.ColCount;
                int matARows = matA.RowCount;

                // A basic matrix multiplication.
                // Parallelize the outer loop to partition the source array by rows.
                Parallel.For(0, matARows, i =>
                {
                    for (int j = 0; j < matBCols; j++)
                    {
                        // Use a temporary to improve parallel performance.
                        double temp = 0;
                        for (int k = 0; k < matACols; k++)
                        {
                            temp += matA[i, k] * matB[k, j];
                        }
                        val[i][j] = temp;
                    }
                });
                Matrix res = new Matrix(val);
                return res;
            }
            else throw new Exception("Wrong matrix dimensions");
        }

    }
}