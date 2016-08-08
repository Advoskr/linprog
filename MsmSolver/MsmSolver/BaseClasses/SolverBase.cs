using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsmSolver.Strategies;

namespace MsmSolver
{
    public abstract class SolverBase
    {
        private readonly IAdditionalTaskHandler _handler;
        public IMathOperationsProvider MathOperationsProvider { get; private set; }

        public SolverBase(IMathOperationsProvider mathOperationsProvider, IAdditionalTaskHandler handler)
        {
            _handler = handler;
            MathOperationsProvider = mathOperationsProvider;
        }

        public Answer SolveTask(Task task)
        {
            var canonicalTask = MakeCanonicalForm(task);
            return SolveTaskInternal(canonicalTask);
        }

        protected Answer SolveTaskInternal(Task task)
        {
            //recreate task to get canonical form without <= / >=
            //now we need to get basis from task
            var getBasisResult = GetBasis(task);
            if (getBasisResult.Item1)
            {
                Basis basis = getBasisResult.Item2;
                return SolveWithBasis(task, basis);
            }
            else
            {
                var additionalTask = _handler.FormAdditionalTask(task);
                var additionalAnswer = SolveTaskInternal(additionalTask);
                return additionalAnswer;
            }
        }

        public Answer SolveWithBasis(Task canonicalTask, Basis basis)
        {
            var x0 = FormX0(basis, canonicalTask);
            var lambda = CalculateLambdas(canonicalTask, basis);

            var solverData = new TaskSolvingData()
            {
                Basis = basis,
                X0 = x0,
                Lambda = lambda,
                //Deltas = deltas,
            };
            var result = SolveWithData(canonicalTask, solverData);
            return result;
        }

        private double CalculateZ(Task task, TaskSolvingData data)
        {
            double result = 0.0d;
            for (int i = 0; i < data.Basis.VectorIndexes.Length; i++)
            {
                result += task.C[data.Basis.VectorIndexes[i]] * data.X0[i];
            }
            return result;
        }

        protected virtual Answer SolveWithData(Task task, TaskSolvingData data)
        {
            var result = new Answer();
            TaskSolvingData newData = new TaskSolvingData();
            newData = data;
            while (true)
            {
                var deltas = CalculateDeltas(task, newData.Basis, newData.Lambda);
                var canBeOptimized = GetCanBeOptimized(deltas);
                if (!canBeOptimized)
                    break;


                var incomingVectorIdx = FindIncomingVector(deltas);
                Vector xs = MathOperationsProvider.Multiply(newData.Basis.Values, task.A.GetColumn(incomingVectorIdx));
                var outgoingVectorIdx = FindOutgoingVector(task, newData, incomingVectorIdx, xs);
                //TODO Merge Xs, out-,in-coming idx and delta into "Step parameters"
                newData = PutVectorIntoBasis(incomingVectorIdx, outgoingVectorIdx, task, newData, deltas, xs);

                //canBeOptimized = GetCanBeOptimized(deltas);

               // Console.WriteLine(string.Format("Vvodim {0} vmesto {1}", incomingVectorIdx, outgoingVectorIdx + task.A.ColCount - task.A.RowCount));
                result.StepCount++;                                         // посколько outgoingVectorIdx - строка, а не номер вектора
            }
            result.Basis = newData.Basis;
            result.Solution = newData.X0;
            result.SolvingMethod = GetSolvingMethodName();
            result.Z = CalculateZ(task, newData);
            return result;
        }

        protected abstract TaskSolvingData PutVectorIntoBasis(int incomingVectorIdx, int outgoingVectorIdx, Task task, TaskSolvingData data, Vector deltas, Vector Xs);

        protected abstract int FindOutgoingVector(Task task, TaskSolvingData data, int incomingVectorIdx, Vector xs);

        protected abstract int FindIncomingVector(Vector deltas);

        private bool GetCanBeOptimized(Vector deltas)
        {
            const double eps = 1e-7d;
            return deltas.Value.Any(t => !double.IsNaN(t) && (Math.Sign(t) == -1 && Math.Abs(t) > eps));
        }

        protected abstract Vector CalculateDeltas(Task task, Basis basis, Vector lambdas);

        protected Vector CalculateLambdas(Task task, Basis basis)
        {
            Vector L = new Vector(basis.Values.ColCount);
            for (int i = 0; i < basis.VectorIndexes.Length; i++)
            {
                L[i] = task.C[basis.VectorIndexes[i]];
            }
            return L;
        }

        protected Vector FormX0(Basis basis, Task task)
        {
            //TODO we don't find real components of X0, we just copy A0 as if our basis is E.
            Vector X0 = MathOperationsProvider.Multiply(basis.Values, task.A0);//Разложение А0 по B,Потом разберусь
            return X0;
        }

        protected Tuple<bool, Basis> GetBasis(Task task)
        {
            Basis basis = new Basis();
            basis.Values = new Matrix(task.A.RowCount, task.A.RowCount, Matrix.CreationVariant.IdentityMatrix);
            basis.VectorIndexes = new int[basis.Values.ColCount];

            bool isBasis = false;
            int indexesIdx = 0;
            for (int j = 0; j < task.A.ColCount; j++)
            {
                int Num_Row = 0;
                for (int i = 0; i < task.A.RowCount; i++)
                {
                    // not identity matrix, so this cannot be canonical basis.
                    if (task.A[i][j] != 1 && task.A[i][j] != 0) { isBasis = false; break; }
                    //found basis value. all other values must be zero
                    if (task.A[i][j] == 1)
                    {
                        //we already had basis value here
                        if (isBasis)
                        { isBasis = false; break; }
                        //this row wasn't basis one, but now it is. 
                        Num_Row = i;
                        isBasis = true;
                    }
                }
                //checked row. Now, if it's basis, we go to this block.
                for (int i = 0; i < task.A.ColCount; i++)
                {
                    if (basis.VectorIndexes.Contains(i) && task.A[Num_Row][i] == task.A[Num_Row][j] && MTaskBasisFinder.Proverka(basis.VectorIndexes))
                    { isBasis = false; break; }
                }

                if (isBasis)
                {
                    //A has an identity vector in it, so we can copy it to out bobr. It'll guarantee that 1 is in correct position.
                    basis.Values.ChangeColumn(indexesIdx, task.A.GetColumn(j));
                    //vector index is our column index.
                    basis.VectorIndexes[indexesIdx++] = j;
                    // set the flag back again
                    isBasis = false;
                }
                //else, we continue to check A matrix. 

            }

            return new Tuple<bool, Basis>(indexesIdx == basis.VectorIndexes.Length, basis);
        }

        public virtual Task MakeCanonicalForm(Task task)
        {
            var result = new Task();

            int counter = 0; //  Сколько ограничений, где не нужна искусственная переменная

            for (int i = 0; i < task.Signs.Length; i++)
            {
                if (task.Signs[i] == Signs.R)
                {
                    counter++;
                }
            }

            var Avals = new double[task.A.RowCount][]; // Будущая матрица result.A;
            result.A0 = new Vector(task.A0.Dimension);
            result.C = new Vector(task.A.ColCount + task.A.RowCount - counter);
            result.Direction = new Direction();
            result.Signs = new Signs[task.A.RowCount];


            for (int i = 0; i < task.A.RowCount; i++)
            {
                Avals[i] = new double[task.A.ColCount + task.A.RowCount - counter];
            }
            int count = 0; // счётчик текущего кол-ва равенств, чтобы знать, в какую позицию тыкать 1 или -1;

            for (int i = 0; i < task.A.RowCount; i++)
            {
                if (task.Signs[i] == Signs.R)
                {
                    count++;
                }
                result.A0[i] = task.A0[i];
                result.Signs[i] = Signs.R;

                for (int j = 0; j < task.A.ColCount + task.A.RowCount - counter; j++)
                {
                    if (j < task.A.ColCount)
                    {
                        Avals[i][j] = task.A[i][j];
                    }

                    else
                    {
                        if (task.Signs[i] == Signs.R) { Avals[i][j] = 0; }
                        if (task.Signs[i] == Signs.M_R && j != task.A.ColCount + i - count)
                        { Avals[i][j] = 0; }
                        if (task.Signs[i] == Signs.M_R && j == task.A.ColCount + i - count)
                        { Avals[i][j] = 1; }
                        if (task.Signs[i] == Signs.B_R && j != task.A.ColCount + i - count)
                        { Avals[i][j] = 0; }
                        if (task.Signs[i] == Signs.B_R && j == task.A.ColCount + i - count)
                        { Avals[i][j] = -1; }
                    }
                }
            }

            result.A = new Matrix(Avals);

            if (task.Direction == (Direction)0)
            {
                result.Direction = (Direction)0;
                for (int i = 0; i < task.A.ColCount + task.A.RowCount - counter; i++)
                {
                    if (i <= task.A.ColCount)
                        result.C[i] = task.C[i];
                    else
                        result.C[i] = 0;
                }
            }
            else
            {
                result.Direction = (Direction)0;
                for (int i = 0; i < task.A.ColCount + task.A.RowCount - counter; i++)
                {
                    if (i <= task.A.ColCount)
                        result.C[i] = -task.C[i];
                    else
                        result.C[i] = 0;
                }
            }

            return result;
        }
        public abstract string GetSolvingMethodName();
    }
}
