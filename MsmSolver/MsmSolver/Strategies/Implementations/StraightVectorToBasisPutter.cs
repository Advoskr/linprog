namespace MsmSolver.Strategies
{
    public class StraightVectorToBasisPutter : IVectorToBasisPutter
    {
        private readonly IMathOperationsProvider _provider;

        public StraightVectorToBasisPutter(IMathOperationsProvider provider)
        {
            _provider = provider;
        }

        private static Vector _eVector = null;
        private static Matrix E = null;
        private static int _numvivodold = 0;
        private static Vector _nullVector = null;

        public TaskSolvingData PutVectorIntoBasis(int incomingVectorIdx, int outgoingVectorIdx, Task task, TaskSolvingData data,
            Vector deltas, Vector xs)
        {
            if (_eVector == null)
                _eVector = new Vector(data.Basis.Values.ColCount);

            if (E == null)
                E = new Matrix(task.A.RowCount, task.A.RowCount, Matrix.CreationVariant.IdentityMatrix);

            if (_nullVector == null)
                _nullVector = new Vector(data.Basis.Values.RowCount);

            var newData = new TaskSolvingData();

            //recalc lambdas
            for (int i = 0; i < data.Lambda.Dimension; i++)
            {
                newData.Lambda[i] -= (data.Basis.Values[outgoingVectorIdx, i] / xs[outgoingVectorIdx]) * deltas[incomingVectorIdx];
            }

            #region recalc Basis.
            //вариант 2
            for (int i = 0; i < E.RowCount; i++)
            {
                _eVector[i] = (i != outgoingVectorIdx) ? ((-1) * xs[i]) / xs[outgoingVectorIdx] : 1 / xs[outgoingVectorIdx];
            }
            _nullVector[_numvivodold] = 1;
            E.ChangeColumn(_numvivodold, _nullVector);
            E.ChangeColumn(outgoingVectorIdx, _eVector);
            _nullVector[_numvivodold] = 0;
            _numvivodold = outgoingVectorIdx;
            var newBasisValues = _provider.Multiply(data.Basis.Values, E);

            newData.Basis.VectorIndexes = data.Basis.VectorIndexes;
            newData.Basis.VectorIndexes[outgoingVectorIdx] = incomingVectorIdx;
            newData.Basis.Values = newBasisValues;
            
            #endregion

            //recalc solution vector
            for (int i = 0; i < data.X0.Dimension; i++)
            {
                if (i != outgoingVectorIdx) data.X0[i] -= (data.X0[outgoingVectorIdx] / xs[outgoingVectorIdx]) * xs[i];
            }

            //Z = 0;
            ////дерьмо какое-то, разберись потом
            //for (int i = 0; i < vectorsIndexes.Length; i++)
            //{
            //    Z += C[vectorsIndexes[i]] * X0[i];
            //}

            return newData;
        }
    }
}