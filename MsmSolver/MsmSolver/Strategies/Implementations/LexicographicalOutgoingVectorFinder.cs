using System;
using System.Linq;

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

    
        public int FindOutgoingVector(Task task, TaskSolvingData data, int incomingVectorIdx, Vector xs)
        {
        
            double minimum = int.MaxValue;
            bool needLeks = false;
            int numvivod = -1;
            for (int k = 0; k < xs.Dimension; k++)
            {
                double val = data.X0[k] / xs[k];


                if (val == minimum)
                {
                    needLeks = true;
                }

                if (val < minimum && xs[k] > 0)
                {
                    numvivod = k;
                    minimum = val;
                    needLeks = false;
                }


            }
            if (needLeks)
            {
                minimum = int.MaxValue;
                for (int i = 0; i < task.A.ColCount; i++)
                {
                    if (data.Basis.VectorIndexes.Contains(i)) continue;
                    Vector column = MathOperationsProvider.Multiply(data.Basis.Values, task.A.GetColumn(i));
                    for (int k = 0; k < xs.Dimension; k++)
                    {
                        double val = column[k] / xs[k];

                        if (val == minimum)
                        {
                            needLeks = true;
                        }

                        if (val < minimum && xs[k] > 0)
                        {
                            numvivod = k;
                            minimum = val;
                            needLeks = false;
                        }

                    }
                    if (!needLeks) break;
                }
            }
            return numvivod;
        }
        //}
        //if (needLeks)
        //{
        //    minimum = int.MaxValue;
        //    for (int i = 0; i < A.ColCount; i++)
        //    {
        //        if (vectorsIndexes.Contains(i)) continue;
        //        Vector column = Bobr*A.GetColumn(i);
        //        for (int k = 0; k < Xs.Dimension; k++)
        //        {
        //            double val = column[k] / Xs[k];

        //            if (val == minimum)
        //            {
        //                needLeks = true;
        //            }

        //            if (val < minimum && Xs[k] > 0)
        //            {
        //                numvivod = k;
        //                minimum = val;
        //                needLeks = false;
        //            }

        //        }
        //        if (!needLeks) break;
        //    }
        //}
    }
}