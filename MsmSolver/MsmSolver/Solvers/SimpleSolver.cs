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
            
            //recalc lambdas
            for (int i = 0; i < data.Lambda.Dimension; i++)
            {
                newData.Lambda[i] -= (data.Basis.Values[outgoingVectorIdx, i] / Xs[outgoingVectorIdx]) * deltas[incomingVectorIdx];
            }

            #region recalc Basis.
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
            var newBasisValues = MathOperationsProvider.Multiply(data.Basis.Values, E);

            newData.Basis.VectorIndexes = data.Basis.VectorIndexes;
            newData.Basis.VectorIndexes[outgoingVectorIdx] = incomingVectorIdx;
            newData.Basis.Values = newBasisValues;
            //Bobr.Math_Mul(E);
            //пересчет обр.баз.матр.
            //вариант 1  
            //for (int i = 0; i < Bobr.RowCount; i++)
            //    for (int j = 0; j < Bobr.ColCount; j++)
            //    {
            //        Bobr[i, j] = (i != numvivod)
            //                         ? Bobr[i, j] - (Bobr[numvivod, j]/Xs[numvivod])*Xs[i]
            //                         : Bobr[i, j]/Xs[numvivod];
            //    }
            #endregion

            //recalc solution vector
            for (int i = 0; i < data.X0.Dimension; i++)
            {
                if (i != outgoingVectorIdx) data.X0[i] -= (data.X0[outgoingVectorIdx] / Xs[outgoingVectorIdx]) * Xs[i];
            }

            //Z = 0;
            ////дерьмо какое-то, разберись потом
            //for (int i = 0; i < vectorsIndexes.Length; i++)
            //{
            //    Z += C[vectorsIndexes[i]] * X0[i];
            //}

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
            //найдем минимум ’ko/Xks и, т.о., выводимый вектор
            var outgoingVectorIdx = -1;
            double minimum = int.MaxValue;
            for (int k = 0; k < Xs.Dimension; k++)
            {
                //TODO Check this. I had A0 in my old impl.
                var newMin = data.X0[k] / Xs[k];
                if (newMin < minimum && Xs[k] > 0)
                {
                    outgoingVectorIdx = k;
                    minimum = newMin;
                }
            }
            return outgoingVectorIdx;
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
                //TODO Replace with "multiply" call
                deltas[i] =
                deltas[i] = lambdas * task.A.GetColumn(i) - task.C[i];

                ////small optimization - break if negative delta found.
                //if (Math.Sign(deltas[i]) == -1 && Math.Abs(deltas[i]) > eps)
                //{
                //    break;
                //}
            }
            return deltas;
        }
        
        protected override Basis GetBasis(Task task)
        {
            //TODO WE make identity matrix here.
            Matrix E = new Matrix(task.A.RowCount, task.A.RowCount, Matrix.CreationVariant.IdentityMatrix);
            //Vector eVector = new Vector(E.ColCount);
            return new Basis()
            {
                Values = E,
                //TODO Bad! Here we just make a sequence (1,2,3,4,etc.), but we need to find real indexes and real bobr
                VectorIndexes = Enumerable.Range(0,task.A.ColCount).ToArray()
            };
        }

        public override string GetSolvingMethodName()
        {
            return "Modified Simplex Method";
        }

    }
}