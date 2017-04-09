namespace MsmSolver.Strategies
{
    public class SimpleOutgoingVectorFinder : IOutgoingVectorFinder
    {
        public int FindOutgoingVector(Task task, TaskSolvingData data, int incomingVectorIdx, Vector xs)
        {
            //найдем минимум Хko/Xks и, т.о., выводимый вектор
            var outgoingVectorIdx = -1;
            double minimum = double.MaxValue;
            //int counter = 0;

           /* for (int i = 0; i < xs.Dimension; i++)
            {
                if (xs[i] <= 0)
                    counter++;
                if (counter == xs.Dimension)
                    throw new System.Exception("CF not ogranichena");
            }*/

            for (int k = 0; k < xs.Dimension; k++)
            {
                if (xs[k] <= 0)
                    continue;
                //TODO Check this. I had A0 in my old impl.
                var newMin = data.X0[k] / xs[k];
                if (newMin < minimum && xs[k] > 0)
                {
                    outgoingVectorIdx = k;
                    minimum = newMin;
                }
            }
            if (outgoingVectorIdx == -1)
                throw new System.Exception("Целевая функция не ограничена");
            return outgoingVectorIdx;
        }
    }
}