using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace MsmSolver.Strategies
{
    public class MulticoreCoreMathOperationsProvider : SingleCoreMathOperationsProvider
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
                    Parallel.For(0, matARowCount, i =>
                    {
                        unchecked
                        {
                            for (int j = 0; j < matBColCount; j++)
                            {
                               // Use a temporary to improve parallel performance.
                               double temp = 0;
                                for (int k = 0; k < matAColCount; k++)
                                {
                                    temp += matA[i, k] * matB[k, j];
                                }
                                val[i][j] = temp;
                            }
                        }
                    });

                    res.Initialize(val);
                    return res;
                }
                else throw new Exception("Wrong matrix dimensions");
            }
        }
    }
}
