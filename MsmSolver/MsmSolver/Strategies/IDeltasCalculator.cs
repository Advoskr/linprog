namespace MsmSolver.Strategies
{
    public interface IDeltasCalculator
    {
        Vector CalculateDeltas(Task task, Basis basis, Vector lambdas);
    }
}