using System;

namespace MsmSolver.Strategies
{

    //TODO needs recheck!!
    public class LexicographicalOutgoingVectorFinder : IOutgoingVectorFinder
    {
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

                if (val < minimum && xs[k] > 0)
                {
                    numvivod = k;
                    minimum = val;
                    needLeks = false;
                }


                if (val == minimum)
                {
                    needLeks = true;
                }


            }
            if (needLeks)
            {
                minimum = int.MaxValue;
                for (int i = 0; i < task.A.ColCount; i++)
                {
                    if (Array.IndexOf(data.Basis.VectorIndexes,i)!=-1) continue;
                    Vector column = data.Basis.Values * task.A.GetColumn(i);
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
    }
}