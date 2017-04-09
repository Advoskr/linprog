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
using MsmSolver.Strategies.Implementations;
using CircleGenerator = MsmSolver.BaseClasses.CircleGenerator;



namespace TestConsoleForEverything
{

    class Program
    {
        static void Main(string[] args)
        {
            /*   //Console.WriteLine(Math.Sign(-0.0014f));

               //RunMultipleIterations();
               const int cols = 300;
               const int rows = 300;
               //const int doubleSize = sizeof(double);
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
               int counter=10;
               for (int i = 0; i < counter; i++)
               {
                   result = mp.Multiply(matrix, matrix);
               }
               Console.WriteLine("Columns:"+result.ColCount);
               sw.Stop();
               Console.WriteLine(sw.Elapsed.TotalMilliseconds);

               var matrix2 = new Matrix(cols, rows);
               for (int i = 0; i < rows; i++)
               {
                   for (int j = 0; j < cols; j++)
                   {
                       matrix2[i,j] = i + j+1;
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
                       if(res[i,j]!=result[i,j])
                           throw new Exception("!");
                   }
               }

               //int counter=10000;
               //for (int i = 0; i < counter; i++)
               //    {
               //{
               //    var cl = new SomeClass(rectArray);
               //sw.Stop();
               //Console.WriteLine(sw.Elapsed.TotalMilliseconds);
               Console.ReadLine();*/

            //   RunMultipleIterations(args[0]);
            //  Console.ReadLine();

            //   var z = new Task();
            //   StreamReader reader = new StreamReader("Zadacha 300x300.txt");
            //  z = new GTaskReader().ReadFromGFile(reader);

            //  Console.ReadLine();

              RunMultipleIterations("Zadacha 300x300.txt");
           /* var z = new CircleGenerator().taskGeneration();

            var mathProvider = new CudaMathOperationsProvider();

            var answer = new ModularSolver(mathProvider, new FullParallelDeltasCalculator(mathProvider), new StraightVectorToBasisPutter(mathProvider),
                         new LastIncomingVectorFinder(), new MTaskBasisFinder(), new LexicographicalOutgoingVectorFinder(mathProvider)).SolveTask(z);

            Console.WriteLine("Znachenie CF: " + answer.Z);
            Console.WriteLine("Kol_vo shagov: " + answer.StepCount);
            Console.WriteLine("Znachenia Икзигов: " + answer.Solution);          
            Console.ReadLine();*/

             Console.ReadLine();
                
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

        private static void RunMultipleIterations(String args)
        {
            List<double> Jenia = new List<double>(5);
            int execCount = 1;
            Answer answer = null;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < execCount; i++)
            {
                answer = ModularSolverCaller(args);
                Jenia.Add(sw.Elapsed.TotalMilliseconds);
                sw.Stop();
               Console.WriteLine($"Iteration: {i} completed");
                sw.Restart();
            }
            Console.Clear();
            Console.WriteLine("Solution: " + answer.Solution);
            Console.WriteLine("Znachenie CF: " + answer.Z);
            Console.WriteLine("Kol_vo shagov: " + answer.StepCount);

            Console.WriteLine("Execution Time:" + Jenia.Average() + "ms");

            Console.ReadLine();

         //  for ( int i = 0; i < answer.Basis.VectorIndexes.GetLength(0);i++)
         //   {
           //    Console.Write(answer.Basis.VectorIndexes[i] + " ");
          //  }
        }

   /*     private static void SolveWithSimpleSolver()
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
        }*/

        public static Answer ModularSolverCaller(string taskFile)
        {

            Task z;
            using (var reader = new StreamReader(taskFile))
            {
                z = new GTaskReader().ReadFromGFile(reader);
                //z = new TaskReader().ReadFromSmallFile(reader);
            }
            var mathProvider = new CudaMathOperationsProvider();

            var answer = new ModularSolver(mathProvider, new FullParallelDeltasCalculator(mathProvider), new StraightVectorToBasisPutter(mathProvider),
                         new LastIncomingVectorFinder(), new MTaskBasisFinder(), new LexicographicalOutgoingVectorFinder(mathProvider)).SolveTask(z);
            return answer;
        }
    }
}