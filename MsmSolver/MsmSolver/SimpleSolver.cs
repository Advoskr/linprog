using System;
using System.Linq;

namespace MsmSolver
{
    public class SimpleSolver : SolverBase
    {
        public SimpleSolver(Func<Matrix, Matrix, Matrix> multiplicationFunctor) : base(multiplicationFunctor)
        {
        }

        protected override TaskSolvingData PutVectorIntoBasis(int incomingVectorIdx, int outgoingVectorIdx, TaskSolvingData data)
        {
            var newData = new TaskSolvingData();
            newData.VectorIndexes = data.VectorIndexes;
            newData.VectorIndexes[outgoingVectorIdx] = incomingVectorIdx;
            throw new NotImplementedException();
            //for (int i = 0; i < L.Dimension; i++)
            //{
            //    L[i] -= (Bobr[outgoingVectorIdx, i] / Xs[outgoingVectorIdx]) * deltas[incomingVectorIdx];
            //}

            return newData;
        }

        protected override int FindOutgoingVector(Task task, TaskSolvingData data, int incomingVectorIdx)
        {
            Vector Xs = data.Basis.Values * task.A.GetColumn(incomingVectorIdx);
            //найдем минимум Хko/Xks и, т.о., выводимый вектор
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

        protected override int FindIncomingVector(TaskSolvingData data)
        {
            for (int i = 0; i < data.Deltas.Dimension; i++)
            {
                if (Math.Sign(data.Deltas[i]) == -1 && Math.Abs(data.Deltas[i]) > eps)
                    return i;
            }
            return -1;
        }


        private const double eps = 1e-7d;
        protected override Vector CalculateDeltas(Task task, Basis basis, Vector lambdas)
        {
            var deltas = new Vector(task.C.Dimension);
            for (int i = 0; i < deltas.Dimension; i++)
            {
                //TODO Replace with "multiply" call
                deltas[i] = lambdas * task.A.GetColumn(i) - task.C[i];
                
                if (Math.Sign(deltas[i]) == -1 && Math.Abs(deltas[i]) > eps)
                {
                    break;
                }
            }
            return deltas;
        }

        protected override Vector CalculateLambdas(Task task, Basis basis)
        {
            Vector L = new Vector(basis.Values.ColCount);
            for (int i = 0; i < basis.VectorIndexes.Length; i++)
            {
                L[i] = task.C[basis.VectorIndexes[i]];
            }
            return L;
        }

        protected override Vector FormX0(Basis basis, Task task)
        {
            //TODO
            Vector X0 = task.A0;//Разложение А0 по B,Потом разберусь
            return X0;
        }

        protected override Basis GetBasis(Task task)
        {
            //TODO WE make identity matrix here.
            Matrix E = new Matrix(task.A.RowCount, task.A.RowCount, Matrix.CreationVariant.IdentityMatrix);
            Vector eVector = new Vector(E.ColCount);
            return new Basis()
            {
                Values = E,
                //TODO Bad! Here we just make a sequence (1,2,3,4,etc.), but we need to find real indexes and real bobr
                VectorIndexes = Enumerable.Range(0,task.A.ColCount).Select(t=>t).ToArray()
            };
        }

        public override string GetSolvingMethodName()
        {
            return "Modified Simplex Method";
        }
    }
}