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

namespace TestConsoleForEverything
{
    class Program
    {
        static void Main(string[] args)
        {
            var z = new Task();
            z = new TaskReader().ReadFromSmallFile(new StreamReader(@"C:\Program Files (x86)\ChetKiyUir\MSM\Additional_Files\Jenia.txt"));
            var mp = new SingleCoreMathOperationsProvider();
            SimpleSolver ss = new SimpleSolver(mp);

            var answer = ss.SolveTask(z);

            Console.WriteLine("Solution: " + answer.Z);

            Console.ReadLine();
        }





        public void ModularSolverCaller(string taskFile)
        {
            Task z;
            using (var reader = new StreamReader(taskFile))
            {
                z = new TaskReader().ReadFromSmallFile(reader);
            }
            var mathProvider = new SingleCoreMathOperationsProvider();
            var answer = new ModularSolver(mathProvider, new FullParallelDeltasCalculator(mathProvider), new StraightVectorToBasisPutter(mathProvider), 
                new FirstIncomingVectorFinder(), new CanonicalInitialBasisFinder(), new SimpleOutgoingVectorFinder())
                .SolveTask(z);
            Console.WriteLine("Solution: "+answer.Solution);
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