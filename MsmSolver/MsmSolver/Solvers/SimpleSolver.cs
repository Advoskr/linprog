using System;
using System.Linq;
using MsmSolver.Strategies;

namespace MsmSolver
{
    public class SimpleSolver : SolverBase
    {

        public SimpleSolver(IMathOperationsProvider mathOperationsProvider) : base(mathOperationsProvider)
        {
        }

        //for optimization purposes
        private static Vector _eVector = null;
        private static Matrix E = null;
        private static int _numvivodold = 0;
        private static Vector _nullVector = null;

        protected override TaskSolvingData PutVectorIntoBasis(int incomingVectorIdx, int outgoingVectorIdx, Task task, TaskSolvingData data, Vector deltas, Vector Xs)
        {
            if(_eVector == null)
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
                _eVector[i] = (i != outgoingVectorIdx) ? ((-1) * Xs[i]) / Xs[outgoingVectorIdx] : 1 / Xs[outgoingVectorIdx];
            }
            _nullVector[_numvivodold] = 1;
            E.ChangeColumn(_numvivodold, _nullVector);
            E.ChangeColumn(outgoingVectorIdx, _eVector);
            _nullVector[_numvivodold] = 0;
            _numvivodold = outgoingVectorIdx;
            var newBasisValues = MathOperationsProvider.Multiply(E, data.Basis.Values);

            newData.Basis.VectorIndexes = data.Basis.VectorIndexes;
            newData.Basis.VectorIndexes[outgoingVectorIdx] = incomingVectorIdx;
            newData.Basis.Values = newBasisValues;
            
            //  recalc solution vector
            newData.X0 = MathOperationsProvider.Multiply(newData.Basis.Values, task.A0);
            
            //recalc Lambdas
            newData.Lambda = MathOperationsProvider.Multiply(GetCbazis(task, newData.Basis), newData.Basis.Values);

            return newData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="data"></param>
        /// <param name="incomingVectorIdx"></param>
        /// <param name="Xs">Components of outgoing vector by basis</param>
        /// <returns></returns>
        protected override int FindOutgoingVector(Task task, TaskSolvingData data, int incomingVectorIdx, Vector Xs)
        {   
            //найдем минимум Хko/Xks и, т.о., выводимый вектор
            var outgoingVectorIdx = -1;
            double minimum = int.MaxValue;
            for (int k = 0; k < Xs.Dimension; k++)
            {
                //TODO Check this. I had A0 in my old impl.
                var newMin = data.X0[k] / Xs[k];
                if (newMin < minimum && Xs[k] > 0)
                {
                    //  outgoingVectorIdx = data.Basis.VectorIndexes[k]; 
                    outgoingVectorIdx = k;
                    minimum = newMin;
                }
            }
            return outgoingVectorIdx;
        }

        protected Vector GetCbazis(Task task, Basis basis)
        {
            var result = new Vector(basis.VectorIndexes.Length);

            for (int i = 0; i < result.Dimension; i++)
            {
                result[i] = task.C[basis.VectorIndexes[i]];
            }

            return result;

        }

        protected const double eps = 1e-7d;
        protected override int FindIncomingVector(Vector deltas)
        {
            for (int i = 0; i < deltas.Dimension; i++)
            {
                if (Math.Sign(deltas[i]) == -1 && Math.Abs(deltas[i]) > eps)
                    return i;
            }
            return -1;
        }

        protected override Vector CalculateDeltas(Task task, Basis basis, Vector lambdas)
        {
            var deltas = new Vector(task.C.Dimension);
            //can be parallel?
            for (int i = 0; i < deltas.Dimension; i++)
            {
                //{
                //    deltas[i] = 0;
                //    continue;
                //}

                deltas[i] = lambdas * task.A.GetColumn(i) - task.C[i];
            }
            return deltas;
        }
        
        protected override Basis GetBasis(Task task)
        {
            //TODO WE make identity matrix here.
            // Matrix E = new Matrix(task.A.RowCount, task.A.RowCount, Matrix.CreationVariant.IdentityMatrix);
            //Vector eVector = new Vector(E.ColCount);
            // return new Basis()
            //  {
            //    Values = E,
            //    //TODO Bad! Here we just make a sequence (1,2,3,4,etc.), but we need to find real indexes and real bobr
            //    VectorIndexes = Enumerable.Range(0,task.A.ColCount).ToArray()
            //};

            CanonicalInitialBasisFinder cf = new CanonicalInitialBasisFinder();
            var result = cf.GetInitialBasis(task);
            return result;
        }

        public override string GetSolvingMethodName()
        {
            return "Modified Simplex Method";
        }

    }
}