using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsmSolver.Strategies.Implementations
{
   public class AdditionalTaskBasisFinder
    {
        public Task AdditionalTask(Task task)
        {

            var z = new Task();
            /*  int mn = C.Dimension + A0.Dimension;
              double[] add_c = new double[mn];
              for (int i = C.Dimension; i < add_c.Count(); i++)
              {
                  add_c[i] = -1;
              }
              //new A values
              double[,] add_A = new double[A.RowCount, mn];
              for (int i = 0; i < A.RowCount; i++)
              {
                  for (int j = 0; j < A.ColCount; j++)
                  {
                      add_A[i, j] = A.Values[i, j];
                  }
              }
              for (int i = 0; i < A.RowCount; i++)
              {
                  add_A[i, A.ColCount + i] = 1;
              }
              Zadacha z = new Zadacha(add_A, add_c, A0.Value);

              z.SolveSimplexAMP();
              //случай 1
              for (int i = C.Dimension; i < z.TaskAnswer.Solution.Dimension; i++)
              {
                  if (z.TaskAnswer.Solution[i] != 0) throw new Exception("Задача не имеет допустимых решений");
              }
              //случай 2

              bool anotherCase = false;
              List<int> additVectorsInedex = new List<int>();
              for (int i = 0; i < z.TaskAnswer.vectorsIndexes.Count(); i++)
              {
                  if (z.TaskAnswer.vectorsIndexes[i] > C.Dimension)
                  {
                      additVectorsInedex.Add(z.TaskAnswer.vectorsIndexes[i]);
                      anotherCase = true;
                  }
              }
              //случай 2а
              if (!anotherCase) return z.TaskAnswer;//ахтунг! тут надо немного форматировать ответ!
              //случай 2b
              else
              {
                  foreach (int index in additVectorsInedex)
                  {
                      Vector xs = z.TaskAnswer.Bobr[index] * A;
                      for (int i = 0; i < xs.Dimension; i++)
                      {
                          if (Math.Abs(xs[i] - 0) < eps)
                          {
                              //Ввести этот вектор в базис 
                          }
                      }
                  }
              }




              return z.TaskAnswer;*/
            return z;
        }
    }
}
