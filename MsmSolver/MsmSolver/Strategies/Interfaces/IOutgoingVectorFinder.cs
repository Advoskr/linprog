namespace MsmSolver.Strategies
{
    public interface IOutgoingVectorFinder
    {
        int FindOutgoingVector(Task task, TaskSolvingData data, int incomingVectorIdx, Vector xs);
    }
}