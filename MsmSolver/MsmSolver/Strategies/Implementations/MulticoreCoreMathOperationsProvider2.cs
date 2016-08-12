using System;
using System.Linq;

namespace MsmSolver.Strategies
{
    public class MulticoreCoreMathOperationsProvider2 : SingleCoreMathOperationsProvider
    {
        public override Matrix Multiply(Matrix matA, Matrix matB)
        {
            unchecked
            {
                if (matA.ColCount == matB.RowCount)
                {
                    Matrix res = new Matrix();
                    double[][] val = new double[matA.RowCount][];
                    for (int i = 0; i < matA.RowCount; i++)
                    {
                        val[i] = new double[matB.ColCount];
                    }
                    int matAColCount = matA.ColCount;
                    int matBColCount = matB.ColCount;
                    int matARowCount = matA.RowCount;

                    // A basic matrix multiplication.
                    // Parallelize the outer loop to partition the source array by RowCount.
                    Topt(matA.Values, matB.Values, val);

                    res.Initialize(val);
                    return res;
                }
                else throw new Exception("Wrong matrix dimensions");
            }
        }

        void Topt(double[][] A, double[][] B, double[][] C)
        {
            var source = Enumerable.Range(0, A.GetLength(0));
            var pquery = from num in source.AsParallel()
                select num;
            pquery.ForAll((e) => Popt(A, B, C, e));
        }
        void Popt(double[][] A, double[][] B, double[][] C, int i)
        {
            double[] iRowA = A[i];
            double[] iRowC = C[i];
            for (int k = 0; k < A.GetLength(0); k++)
            {
                double[] kRowB = B[k];
                double ikA = iRowA[k];
                for (int j = 0; j < B.GetLength(0); j++)
                {
                    iRowC[j] += ikA * kRowB[j];
                }
            }
        }
    }
}