namespace MsmSolver.Strategies
{
    public interface IInitialBasisFinder
    {
        Basis GetInitialBasis(Task task);
    }
}