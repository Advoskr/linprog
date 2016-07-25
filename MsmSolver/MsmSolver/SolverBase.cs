using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsmSolver
{
    public abstract class SolverBase
    {
        public SolverBase(Func<Matrix, Matrix, Matrix> multiplicationFunctor)
        {
            MultiplicationFunctor = multiplicationFunctor;
        }

        protected Func<Matrix, Matrix, Matrix> MultiplicationFunctor { get; private set; }

        public Answer SolveTask(Task task)
        {
            //recreate task to get canonical form without <= / >=
            var canonicalTask = MakeCanonicalForm(task);
            //now we need to get basis from task
            var basis = GetBasis(canonicalTask);
            var x0 = FormX0(basis, canonicalTask);
            var lambda = CalculateLambdas(canonicalTask, basis);
            var deltas = CalculateDeltas(canonicalTask, basis, lambda);
            var solverData = new TaskSolvingData()
            {
                Basis = basis,
                X0 = x0,
                Lambda = lambda,
                Deltas = deltas,
            };
            var result = InternalSolve(canonicalTask, solverData);
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

        protected virtual Answer InternalSolve(Task task, TaskSolvingData data)
        {
            var canBeOptimized = GetCanBeOptimized(data.Deltas);
            var result = new Answer();
            TaskSolvingData newData = data;
            while (canBeOptimized)
            {
                var incomingVectorIdx = FindIncomingVector(data);
                var outgoingVectorIdx = FindOutgoingVector(task, newData, incomingVectorIdx);
                
                newData = PutVectorIntoBasis(incomingVectorIdx, outgoingVectorIdx, newData);

                canBeOptimized = GetCanBeOptimized(newData.Deltas);
                result.StepCount++;
            }
            result.Basis = newData.Basis;
            result.Solution = newData.X0;
            result.SolvingMethod = GetSolvingMethodName();
            result.Z = CalculateZ(task, newData);
            return result;
        }

        protected abstract TaskSolvingData PutVectorIntoBasis(int incomingVectorIdx, int outgoingVectorIdx, TaskSolvingData data);

        protected abstract int FindOutgoingVector(Task task, TaskSolvingData data, int incomingVectorIdx);

        protected abstract int FindIncomingVector(TaskSolvingData data);

        private bool GetCanBeOptimized(Vector deltas)
        {
            return deltas.Value.Any(t => t < 0.0);
        }

        protected abstract Vector CalculateDeltas(Task canonicalTask, Basis basis, Vector lambdas);

        protected abstract Vector CalculateLambdas(Task task, Basis basis);

        protected abstract Vector FormX0(Basis basis, Task task);

        protected abstract Basis GetBasis(Task task);

        protected virtual Task MakeCanonicalForm(Task task)
        {
            throw new NotImplementedException();
        }

        public abstract string GetSolvingMethodName();
    }
}
