using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsmSolver.Strategies.Implementations
{
    public class CasualDeltasCalculator : IDeltasCalculator
    {
        private readonly IMathOperationsProvider _provider;

        public CasualDeltasCalculator(IMathOperationsProvider provider)
        {
            _provider = provider;
        }
        public Vector CalculateDeltas(Task task, Basis basis, Vector lambdas)
        {
            var deltas = new Vector(task.C.Dimension);

            for (int i = 0; i < deltas.Dimension; i++)
            {
                if (basis.VectorIndexes.Contains(i))
                { deltas[i] = 0; }
                else
                { deltas[i] = _provider.Multiply(StraightVectorToBasisPutter.GetCbazis(task, basis), basis.Values) * task.A.GetColumn(i) - task.C[i]; }

            }
            return deltas;
        }
    }
}
