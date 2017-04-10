using System;

namespace MsmSolver.Strategies
{

    //TODO needs recheck!!
    public class LexicographicalOutgoingVectorFinder : IOutgoingVectorFinder
    { 
        private readonly IMathOperationsProvider MathOperationsProvider;



        public LexicographicalOutgoingVectorFinder(IMathOperationsProvider mathOperationsProvider)
        {
            MathOperationsProvider = mathOperationsProvider;
        }


        private const double eps = 1e-7d;

        public int FindOutgoingVector(Task task, TaskSolvingData data, int incomingVectorIdx, Vector xs)
        {
            //лексикографический подход
            //для вектора А0
            double minimum = double.MaxValue;
            bool needLeks = false;
            int numvivod = -1;
            int LexVectIndex = 0;
            
            int[] indexValues = new int[xs.Dimension];
            int counter = 0;

            for (int i = 0; i < xs.Dimension; i++)
            {
                if (xs[i] <= 0)
                    counter++;
                if (counter == xs.Dimension)
                    throw new System.Exception("CF not ogranichena");
            }

            for (int k = 0; k < xs.Dimension; k++)
            {
                if (xs[k] < eps) 
                    continue;           
                if (Math.Abs(data.X0[k]) < eps)
                    data.X0[k] = 0;

                double val = data.X0[k] / xs[k];

               // if (val >= int.MaxValue)
                //    throw new Exception("WHAT BLIN?");

                if (Math.Abs(val - minimum) < eps)
                {
                    LexVectIndex++;
                    needLeks = true;
                }

                if (Math.Abs(val - minimum) >= eps && val < minimum  && xs[k] > 0)
                {
                    numvivod = k;
                    minimum = val;
                    needLeks = false;
                    LexVectIndex = 0;
                }
                
            }
            if (needLeks)
            {
                Console.WriteLine("Alternatives: {0}", LexVectIndex);
                minimum = double.MaxValue;
                for (int i = 0; i < task.A.ColCount; i++)
                {
                    if (Array.IndexOf(data.Basis.VectorIndexes, i) != -1) continue;
                    Vector column = MathOperationsProvider.Multiply(data.Basis.Values, task.A.GetColumn(i));
                    for (int k = 0; k < xs.Dimension; k++)
                    {
                        if (xs[k] < eps || (column[k] < 0 && Math.Abs(column[k]) >= eps))
                                continue;

                        if (Math.Abs(column[k]) < eps)
                            column[k] = 0;

                        double val = column[k] / xs[k];

                        if (Math.Abs(val - minimum) < eps)
                        {
                            needLeks = true;
                        }

                        if (Math.Abs(val - minimum) >= eps && val < minimum && xs[k] > 0)
                        {
                            numvivod = k;
                            minimum = val;
                            needLeks = false;
                        }
                    }
                    if (!needLeks) break;
                }
            }
            if (numvivod == -1)
                throw new Exception("Целевая функция не ограничена");
            return numvivod;
        }
    }
}