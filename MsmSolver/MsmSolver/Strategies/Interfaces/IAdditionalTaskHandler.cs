namespace MsmSolver.Strategies
{
    public interface IAdditionalTaskHandler
    {
        Task FormAdditionalTask(Task initialTask);

        Answer AnalyzeAnswer(Answer answer, SolverBase solver);
    }
}