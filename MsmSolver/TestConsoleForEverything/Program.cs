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

            RunMultipleIterations();

            //const int cols = 300;
            //const int rows = 300;
            //const int doubleSize = sizeof(double);

            //double[,] rectArray = new double[rows, cols];
            //for (int i = 0; i < rows; i++)
            //{
            //    for (int j = 0; j < cols; j++)
            //    {
            //        rectArray[i, j] = i + j;
            //    }
            //}
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

            Console.ReadLine();
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
            var mathProvider = new MulticoreCoreMathOperationsProvider();
            
            var answer = new ModularSolver(mathProvider, new FullParallelDeltasCalculator(mathProvider), new StraightVectorToBasisPutter(mathProvider),
                new FirstIncomingVectorFinder(), new MTaskBasisFinder(), new SimpleOutgoingVectorFinder())
                .SolveTask(z);
            
            return answer;
        }
    }
}