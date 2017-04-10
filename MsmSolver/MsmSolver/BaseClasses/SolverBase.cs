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
        private static bool changeCF = false;
        private static int colvo_perem = 0;

        public IMathOperationsProvider MathOperationsProvider { get; private set; }

        public SolverBase(IMathOperationsProvider mathOperationsProvider, IAdditionalTaskHandler handler)
        {
            _handler = handler;
            MathOperationsProvider = mathOperationsProvider;
        }

        public Answer SolveTask(Task task)
        {
            colvo_perem = task.C.Dimension;
            var canonicalTask = MakeCanonicalForm(task);
            return SolveTaskInternal(canonicalTask);
        }

        protected Answer SolveTaskInternal(Task task)
        {
            //recreate task to get canonical form without <= / >=
            //now we need to get basis from task
            bool JeniaProverka = false;
           
            var getBasisResult = GetBasis(task);    

            JeniaProverka = TestBasis(getBasisResult.Item2.Values);
            
            if (getBasisResult.Item1)
            {
                Basis basis = getBasisResult.Item2;
                basis = RightForm(basis);
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
            
        private bool TestBasis(Matrix matrix)
        {
            for (int i = 0; i < matrix.ColCount; i++)
            {
                for (int j = i + 1; j < matrix.ColCount; j++)
                {
                    if (matrix.GetColumn(i) == matrix.GetColumn(j))
                    { return true; }
                        
                }
            }

            return false;

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

            //bool jenia_Test = false; Проверка на ограниченность ЦФ
            TaskSolvingData newData = new TaskSolvingData();
            newData = data;
            while (true)
            {
                var deltas = CalculateDeltas(task, newData.Basis, newData.Lambda);
                var canBeOptimized = GetCanBeOptimized(deltas);
                if (!canBeOptimized)
                    break;
                

                var incomingVectorIdx = FindIncomingVector(deltas);
              
                Vector xs;
               /* if (result.StepCount == 0)
                    xs = task.A.GetColumn(incomingVectorIdx);
                else
                  */ xs = MathOperationsProvider.Multiply(newData.Basis.Values, task.A.GetColumn(incomingVectorIdx));
                
          /*      for (int i = 0; i < xs.Dimension; i++)
                {
                    if (Math.Sign(xs[i]) != -1)                   Проверка на ограниченность ЦФ
                        jenia_Test = true;
                }

                if (jenia_Test == false)
                    throw new Exception("Целевая функция не ограничена");*/
                var outgoingVectorIdx = FindOutgoingVector(task, newData, incomingVectorIdx, xs);
                
                //TODO Merge Xs, out-,in-coming idx and delta into "Step parameters"
                newData = PutVectorIntoBasis(incomingVectorIdx, outgoingVectorIdx, task, newData, deltas, xs);

                //canBeOptimized = GetCanBeOptimized(deltas);

                
                result.StepCount++;                                         // посколько outgoingVectorIdx - строка, а не номер вектора
            }

            for (int i = 0; i < newData.Basis.VectorIndexes.GetLength(0); i++)
            {
                if (task.C[newData.Basis.VectorIndexes[i]] == -int.MaxValue)
                {
                    Console.WriteLine(result.StepCount);
                    throw new Exception("Task have no solution");
                }                                  
            }
            result.Basis = newData.Basis;
            result.Solution = new Vector(colvo_perem);
           for (int i = 0; i < colvo_perem; i++)
            {
                for (int j = 0; j < result.Basis.VectorIndexes.Length; j++ )
                {
                    if (i == result.Basis.VectorIndexes[j])
                    {
                        result.Solution[i] = newData.X0[j];
                        break;
                    }
                }
            }
            result.SolvingMethod = GetSolvingMethodName();
            result.Z = CalculateZ(task, newData);
            if (changeCF)
                result.Z = -result.Z;
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
           //   Vector L = new Vector(basis.Values.ColCount);
             // for (int i = 0; i < basis.VectorIndexes.Length; i++)
             //// {
              //    L[i] = task.C[basis.VectorIndexes[i]];
             // }
              //return L;
            Vector L = new Vector(basis.Values.ColCount);
            L = MathOperationsProvider.Multiply(StraightVectorToBasisPutter.GetCbazis(task, basis), basis.Values);

            return L;
        }

        protected Vector FormX0(Basis basis, Task task)
        {
            //TODO we don't find real components of X0, we just copy A0 as if our basis is E.
            Vector X0 = MathOperationsProvider.Multiply(basis.Values, task.A0);//Разложение А0 по B,Потом разберусь
            return X0;
        }

        protected Basis RightForm(Basis basis)
        {
            var b = new Basis();

            b.Values = new Matrix(basis.Values.RowCount, basis.Values.ColCount, Matrix.CreationVariant.IdentityMatrix);
            b.VectorIndexes = new int[basis.Values.RowCount];

            for (int i = 0; i < basis.Values.RowCount; i++)
            {
                for (int j = 0; j < basis.Values.ColCount; j++)
                {
                    if (basis.Values[i][j] == 1)
                    {
                        b.VectorIndexes[i] = basis.VectorIndexes[j];
                    }
                }
            }
            return b;
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
                    if (basis.VectorIndexes.Contains(i) && task.A[Num_Row][i] == task.A[Num_Row][j] && MTaskBasisFinder.Proverka(basis.VectorIndexes) && basis.VectorIndexes[task.A.RowCount - 1] != 0)
                    { isBasis = false; break; }
                }

                if (isBasis)
                {
                    //A has an identity vector in it, so we can copy it to out bobr. It'll guarantee that 1 is in correct position.
                    basis.Values.ChangeColumn(indexesIdx, task.A.GetColumn(j));
                    //vector index is our column index.
                    basis.VectorIndexes[indexesIdx] = j;
                    indexesIdx++;
                    // set the flag back again
                    isBasis = false;
                }
                //else, we continue to check A matrix. 

            }


           /*  if (indexesIdx == basis.VectorIndexes.Length)
            {
                for (int i = 0; i < basis.Values.ColCount; i++)
                {
                    if (basis.Values[i][i] == 0)
                    {
                        for (int j = 0; j < basis.Values.RowCount; j++)
                        {
                            if (basis.Values[i][j] == 1)
                            {

                            }
                        }

                    }
                    else continue;
                }
                
            }*/



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
                changeCF = true;
                for (int i = 0; i < task.A.ColCount + task.A.RowCount - counter; i++)
                {
                    if (i <= task.A.ColCount)
                        result.C[i] = -task.C[i];
                    else
                        result.C[i] = 0;
                }
            }

            for (int i = 0; i < result.A.RowCount; i++)    // Неотрицательность А0
            {
                if (Math.Sign(result.A0[i]) == -1)
                {
                    result.A0[i] = -result.A0[i];
                    for (int j = 0; j < result.A.ColCount; j++)
                    {
                        result.A._values[i][j] = -result.A._values[i][j];
                    }
                }
                else
                    continue;
            }


            return result;
        }
        public abstract string GetSolvingMethodName();
    }
}
