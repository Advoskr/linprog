using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsmSolver.Strategies
{
   public class MulticoreCoreMathOperationsProvider : SingleCoreMathOperationsProvider
    {
        public override Matrix Multiply(Matrix matA, Matrix matB)
        {
            if (matA.ColCount == matB.RowCount)
            {
                Matrix res = new Matrix();
                double[][] val = new double[matA.RowCount][];
                for (int i = 0; i < matA.RowCount; i++)
                {
                    val[i] = new double[matB.ColCount];
                }
                int matACols = matA.ColCount;
                int matBCols = matB.ColCount;
                int matARows = matA.RowCount;

                // A basic matrix multiplication.
                // Parallelize the outer loop to partition the source array by rows.
                Parallel.For(0, matARows, i =>
                {
                    for (int j = 0; j < matBCols; j++)
                    {
                        // Use a temporary to improve parallel performance.
                        double temp = 0;
                        for (int k = 0; k < matACols; k++)
                        {
                            temp += matA[i, k] * matB[k, j];
                        }
                        val[i][j] = temp;
                    }
                });

                res.Initialize(val);
                return res;
            }
            else throw new Exception("Wrong matrix dimensions");
        }
    }
}
