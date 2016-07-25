using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsmSolver
{
    public abstract class SolverBase
    {
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
            //TODO Ugly, rethink. Z is part of an answer, not solving data.
            solverData.Z = CalculateZ(canonicalTask, solverData);
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

        protected virtual Answer InternalSolve(Task canonicalTask, TaskSolvingData data)
        {
            var canBeOptimized = GetCanBeOptimized(data.Deltas);
            var result = new Answer();
            while (canBeOptimized)
            {
                var outgoingVectorIdx = FindOutgoingVector();
                var incomingVectorIdx = FindIncomingVector();
                var newData = PutVectorIntoBasis(incomingVectorIdx, outgoingVectorIdx, data);

                canBeOptimized = GetCanBeOptimized(newData.Deltas);
            }
            return result;
        }

        private TaskSolvingData PutVectorIntoBasis(int incomingVectorIdx, int outgoingVectorIdx, TaskSolvingData data)
        {
            throw new NotImplementedException();
        }

        protected abstract int FindOutgoingVector();

        protected abstract int FindIncomingVector();

        private bool GetCanBeOptimized(Vector deltas)
        {
            return deltas.Value.Any(t => t < 0.0);
        }

        protected abstract Vector CalculateDeltas(Task canonicalTask, Basis basis, Vector lambda);

        protected abstract Vector CalculateLambdas(Task canonicalTask, Basis basis);

        protected abstract Vector FormX0(Basis basis, Task canonicalTask);

        protected abstract Basis GetBasis(Task task);

        private Task MakeCanonicalForm(Task task)
        {
            throw new NotImplementedException();
        }
    }
}
