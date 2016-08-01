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
            RunMultipleIterations();
            Console.ReadLine();
        }

        private static void RunMultipleIterations()
        {
            List<double> Jenia = new List<double>(5);
            int execCount = 5;
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
            var mathProvider = new CudaMathOperationsProvider();
            
            var answer = new ModularSolver(mathProvider, new FullParallelDeltasCalculator(mathProvider), new StraightVectorToBasisPutter(mathProvider),
                new FirstIncomingVectorFinder(), new MTaskBasisFinder(), new SimpleOutgoingVectorFinder())
                .SolveTask(z);
            
            return answer;
        }
    }
}