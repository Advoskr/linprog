using System;
using System.Threading.Tasks;

namespace MsmSolver.Strategies.Implementations
{
    public class MultiCoreStrassenMathOperationsProvider: MulticoreCoreMathOperationsProvider
    {
        public override Matrix Multiply(Matrix matA, Matrix matB)
        {
            return base.Multiply(matA, matB);
        }

        private const int MinMatrixSize = 32;

        public Matrix VinogradovMultiplyMatrix(Matrix A, Matrix B)
        {
            return new Matrix();
        }
    }
}