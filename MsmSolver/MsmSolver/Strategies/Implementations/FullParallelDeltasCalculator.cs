﻿using System.Threading.Tasks;
using System.Linq;

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
                if (basis.VectorIndexes.Contains(i))
                    deltas[i] = 0;
                else
                deltas[i] = _provider.Multiply(lambdas, task.A.GetColumn(i)) - task.C[i];
            });
            
            return deltas;
        }
    }
}