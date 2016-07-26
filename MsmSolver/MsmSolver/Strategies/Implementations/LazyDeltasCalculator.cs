using System;
using System.Threading.Tasks;

namespace MsmSolver.Strategies
{
    /// <summary>
    /// Calculates deltas only till first negative is found
    /// </summary>
    public class LazyDeltasCalculator : IDeltasCalculator
    {
        private readonly IMathOperationsProvider _provider;

        protected const double eps = 1e-7d;

        public LazyDeltasCalculator(IMathOperationsProvider provider)
        {
            _provider = provider;
        }

        public Vector CalculateDeltas(Task task, Basis basis, Vector lambdas)
        {
            var deltas = new Vector(task.C.Dimension);
            Parallel.For(0, deltas.Dimension, (i, loopState) =>
            {
                deltas[i] = _provider.Multiply(lambdas, task.A.GetColumn(i)) - task.C[i];
                if (Math.Sign(deltas[i]) == -1 && Math.Abs(deltas[i]) > eps)
                    loopState.Break();
            });

            return deltas;
        }
    }
}