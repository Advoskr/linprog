﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsmSolver.Strategies;

namespace MsmSolver
{
    public abstract class SolverBase
    {
        public IMathOperationsProvider MathOperationsProvider { get; private set; }

        public SolverBase(IMathOperationsProvider mathOperationsProvider)
        {
            MathOperationsProvider = mathOperationsProvider;
        }

        public Answer SolveTask(Task task)
        {
            //recreate task to get canonical form without <= / >=
            var canonicalTask = MakeCanonicalForm(task);
            //now we need to get basis from task
            var basis = GetBasis(canonicalTask);
            var x0 = FormX0(basis, canonicalTask);
            var lambda = CalculateLambdas(canonicalTask, basis);
            
            var solverData = new TaskSolvingData()
            {
                Basis = basis,
                X0 = x0,
                Lambda = lambda,
                //Deltas = deltas,
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
                Vector xs = newData.Basis.Values * task.A.GetColumn(incomingVectorIdx);
                var outgoingVectorIdx = FindOutgoingVector(task, newData, incomingVectorIdx, xs);
                //TODO Merge Xs, out-,in-coming idx and delta into "Step parameters"
                newData = PutVectorIntoBasis(incomingVectorIdx, outgoingVectorIdx, task, newData, deltas, xs);

                //canBeOptimized = GetCanBeOptimized(deltas);
                result.StepCount++;
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
            return deltas.Value.Any(t => t < 0.0);
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
            Vector X0 = basis.Values*task.A0;//Разложение А0 по B,Потом разберусь
            return X0;
        }

        protected abstract Basis GetBasis(Task task);

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
