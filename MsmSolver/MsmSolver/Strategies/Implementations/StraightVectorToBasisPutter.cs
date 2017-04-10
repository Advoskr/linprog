using System;
namespace MsmSolver.Strategies
 
{
    public class StraightVectorToBasisPutter : IVectorToBasisPutter
    {
        private readonly IMathOperationsProvider _provider;
        private const double eps = 1e-7d;

        public StraightVectorToBasisPutter(IMathOperationsProvider provider)
        {
            _provider = provider;
        }

        private static Vector _eVector = null;
        private static Matrix E = null;
        private static int _numvivodold = 0;
        private static Vector _nullVector = null;
        private static int counter = 0;

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

            newData.Basis = new Basis();
            newData.X0 = new Vector(data.X0.Dimension);
            newData.Lambda = new Vector(data.Lambda.Dimension);

            // recalc Basis.
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
            var newBasisValues = _provider.Multiply(E,data.Basis.Values);

            Console.WriteLine(string.Format("Vvodim {0} vmesto {1}", incomingVectorIdx, data.Basis.VectorIndexes[outgoingVectorIdx]));

            newData.Basis.VectorIndexes = data.Basis.VectorIndexes;
            newData.Basis.VectorIndexes[outgoingVectorIdx] = incomingVectorIdx;
            newData.Basis.Values = newBasisValues;

            //    var jenia = MatrixTest(newData, task);
            var test = MatrixTest(newData, task);
            if (test.Item1)
                throw new Exception("ALARM VOLK UNES ZAICHAT");

            //  recalc solution vector
            newData.X0 = _provider.Multiply(newData.Basis.Values, task.A0);

            //recalc Lambdas
            newData.Lambda = _provider.Multiply(GetCbazis(task, newData.Basis), newData.Basis.Values);

            return newData;
        }


        public Tuple<bool,Matrix> MatrixTest(TaskSolvingData Newdata, Task Task)
        {
            var test = false;
            var newData = Newdata;
            var task = Task;

            Matrix straightMatrix = new Matrix(newData.Basis.Values.RowCount, newData.Basis.Values.ColCount, Matrix.CreationVariant.IdentityMatrix);
            for (int i = 0; i < straightMatrix.ColCount; i++)
            {
                straightMatrix.ChangeColumn(i, task.A.GetColumn(newData.Basis.VectorIndexes[i]));
            }

            var Jenia = _provider.Multiply(newData.Basis.Values, straightMatrix);

            for (int i = 0; i < Jenia.RowCount; i++)
            {
                for (int j = 0; j < Jenia.ColCount; j++)
                {
                    if (i == j && Math.Abs(Jenia._values[i][j] - 1) > eps || i != j && Math.Abs(Jenia._values[i][j]) > eps)
                    {
                        test = true;
                        return new Tuple<bool, Matrix>(test, straightMatrix);
                    }
                }
            }
            return new Tuple<bool, Matrix>(test, straightMatrix);
        }




        public static Vector GetCbazis(Task task, Basis basis)
        {
            var result = new Vector(basis.VectorIndexes.Length);

            for (int i = 0; i < result.Dimension; i++)
            {
                result[i] = task.C[basis.VectorIndexes[i]];
            }

            return result;

        }

    }
}