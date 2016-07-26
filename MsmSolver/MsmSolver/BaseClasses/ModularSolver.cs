using System;
using MsmSolver.Strategies;

namespace MsmSolver
{
    public class ModularSolver : SolverBase
    {
        private readonly IDeltasCalculator _deltasCalculator;
        private readonly IVectorToBasisPutter _putter;
        private readonly IIncomingVectorFinder _incomingVectorFinder;
        private readonly IInitialBasisFinder _initialBasisFinder;
        private readonly IOutgoingVectorFinder _outgoingVectorFinder;

        public ModularSolver(IMathOperationsProvider mathOperationsProvider, IDeltasCalculator deltasCalculator, 
            IVectorToBasisPutter putter, IIncomingVectorFinder incomingVectorFinder, 
            IInitialBasisFinder initialBasisFinder, IOutgoingVectorFinder outgoingVectorFinder) : base(mathOperationsProvider)
        {
            _deltasCalculator = deltasCalculator;
            _putter = putter;
            _incomingVectorFinder = incomingVectorFinder;
            _initialBasisFinder = initialBasisFinder;
            _outgoingVectorFinder = outgoingVectorFinder;
        }
        
        protected override TaskSolvingData PutVectorIntoBasis(int incomingVectorIdx, int outgoingVectorIdx, 
            Task task, TaskSolvingData data,
            Vector deltas, Vector Xs)
        {
            return _putter.PutVectorIntoBasis(incomingVectorIdx, outgoingVectorIdx, task, data, deltas, Xs);
        }

        protected override int FindOutgoingVector(Task task, TaskSolvingData data, int incomingVectorIdx, Vector xs)
        {
            return _outgoingVectorFinder.FindOutgoingVector(task, data, incomingVectorIdx, xs);
        }

        protected override int FindIncomingVector(Vector deltas)
        {
            return _incomingVectorFinder.FindIncomingVector(deltas);
        }

        protected override Vector CalculateDeltas(Task task, Basis basis, Vector lambdas)
        {
            return _deltasCalculator.CalculateDeltas(task, basis, lambdas);
        }
        
        protected override Basis GetBasis(Task task)
        {
            return _initialBasisFinder.GetInitialBasis(task);
        }

        public override string GetSolvingMethodName()
        {
            //TODO Return info about applied strategies
            return "Modular solver";
        }

    }
}