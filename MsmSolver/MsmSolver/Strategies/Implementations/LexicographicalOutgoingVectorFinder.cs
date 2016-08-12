﻿using System;

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


        private const double eps = 0.0000001;

        public int FindOutgoingVector(Task task, TaskSolvingData data, int incomingVectorIdx, Vector xs)
        {
            //лексикографический подход
            //для вектора А0
            double minimum = int.MaxValue;
            bool needLeks = false;
            int numvivod = -1;
            for (int k = 0; k < xs.Dimension; k++)
            {
                double val = data.X0[k] / xs[k];

                if(xs[k]<=0)
                    continue;

                if (Math.Abs(val - minimum) < eps)
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
                    if (Array.IndexOf(data.Basis.VectorIndexes, i) != -1) continue;
                    Vector column = MathOperationsProvider.Multiply(data.Basis.Values, task.A.GetColumn(i));
                    for (int k = 0; k < xs.Dimension; k++)
                    {
                        if (xs[k] <= 0)
                            continue;

                        double val = column[k] / xs[k];

                        if (Math.Abs(val - minimum) < eps)
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
    }
}