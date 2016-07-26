using System.Threading.Tasks;

namespace MsmSolver.Strategies
{
    public class FullParallelDeltasCalculator : IDeltasCalculator
    {
        private readonly IMathOperationsProvider _provider;

        public FullParallelDeltasCalculator(IMathOperationsProvider provider)
        {
            _provider = provider;
        }

        public Vector CalculateDeltas(Task task, Basis basis, Vector lambdas)
        {
            var deltas = new Vector(task.C.Dimension);
            Parallel.For(0, deltas.Dimension, i =>
            {
                deltas[i] = _provider.Multiply(lambdas, task.A.GetColumn(i)) - task.C[i];
            });
            
            return deltas;
        }
    }
}