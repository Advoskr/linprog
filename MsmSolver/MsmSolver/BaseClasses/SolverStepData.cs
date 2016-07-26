namespace MsmSolver.BaseClasses
{
    public class SolverStepData
    {
        public int IncomingVectorIndex { get; set; }
        public int OutgoingVectorIndex { get; set; }
        public Vector DeltasVector { get; set; }
        public Vector XsVector { get; set; }
    }
}