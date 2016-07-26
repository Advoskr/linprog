namespace MsmSolver.Strategies
{
    public class SimpleOutgoingVectorFinder : IOutgoingVectorFinder
    {
        public int FindOutgoingVector(Task task, TaskSolvingData data, int incomingVectorIdx, Vector xs)
        {
            //найдем минимум Хko/Xks и, т.о., выводимый вектор
            var outgoingVectorIdx = -1;
            double minimum = int.MaxValue;
            for (int k = 0; k < xs.Dimension; k++)
            {
                //TODO Check this. I had A0 in my old impl.
                var newMin = data.X0[k] / xs[k];
                if (newMin < minimum && xs[k] > 0)
                {
                    outgoingVectorIdx = k;
                    minimum = newMin;
                }
            }
            return outgoingVectorIdx;
        }
    }
}