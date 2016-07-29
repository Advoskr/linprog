namespace MsmSolver.Strategies
{
    public interface IMathOperationsProvider
    {
        Matrix Multiply(Matrix left, Matrix right);

        Matrix Divide(Matrix left, Matrix right);

        Matrix Sum(Matrix left, Matrix right);

        double Multiply(Vector left, Vector right);

        Vector Sum(Vector left, Vector right);

        Vector Subtract(Vector left, Vector right);

        Vector Divide(Vector left, Vector right);

        Vector Multiply(Vector left, Matrix right);

        Vector Multiply(Matrix left, Vector right);
    }
}