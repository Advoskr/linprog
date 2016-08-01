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

          var answer = ModularSolverCaller("Zadacha 300x300.txt");

            Console.WriteLine("Solution: " + answer.Solution);
            Console.WriteLine("z: " + answer.Z);
            Console.WriteLine("Kol_vo shagov: " + answer.StepCount);

            Console.ReadLine();
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
            var sw = Stopwatch.StartNew();
            var answer = new ModularSolver(mathProvider, new FullParallelDeltasCalculator(mathProvider), new StraightVectorToBasisPutter(mathProvider),
                new FirstIncomingVectorFinder(), new MTaskBasisFinder(), new SimpleOutgoingVectorFinder())
                .SolveTask(z);
            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            Jenia.Add(sw.Elapsed.TotalMilliseconds);
            return answer;
        }
    }
}