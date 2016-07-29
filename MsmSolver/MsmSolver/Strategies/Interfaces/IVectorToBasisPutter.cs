using MsmSolver.BaseClasses;

namespace MsmSolver.Strategies
{
    public interface IVectorToBasisPutter
    {
        TaskSolvingData PutVectorIntoBasis(int incomingVectorIdx, int outgoingVectorIdx, Task task, TaskSolvingData data, Vector deltas, Vector xs);
    }
}