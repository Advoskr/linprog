using System;

namespace MsmSolver.Strategies
{
    public class FirstIncomingVectorFinder : IIncomingVectorFinder
    {
        protected const double eps = 1e-7d;
        public int FindIncomingVector(Vector deltas)
        {
            for (int i = 0; i < deltas.Dimension; i++)
            {
                if (Math.Sign(deltas[i]) == -1 && Math.Abs(deltas[i]) > eps)
                    return i;
            }
            return -1;
        }
    }
}
/*public class LastIncomingVectorFinder : IIncomingVectorFinder
{
    protected const double eps = 1e-7d;
    public int FindIncomingVector(Vector deltas)
    {
        var result = -1;
        for (int i = 0; i < deltas.Dimension; i++)
        {
            if (Math.Sign(deltas[i]) == -1 && Math.Abs(deltas[i]) > eps)
                result = i;
        }
        return result;
    }
}*/