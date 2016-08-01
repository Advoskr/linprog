using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MsmSolver.Strategies
{
    public class CudaMathOperationsProvider : SingleCoreMathOperationsProvider
    {
        /*

          public void Math_Mul_Tile(Matrix a)
          {
              double[,] vC = new double[a.ColCount, this.RowCount];
              externExtensions.MatMulFire(vC, a._values, this._values, a.RowCount, this.ColCount, a.ColCount);
              _values = vC;
          }*/

        private double[,] Transformer1_2(double[][] ss)
            {
            double[,] matrix = new double[ss.GetLength(0), ss[0].GetLength(0)];

                for (int i = 0; i < matrix.GetLength(0); i++)
                    {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                        {
                            matrix[i,j] = ss[i][j];
                        }
                    }
                return matrix;
            }
        
        private double[][] Transformer2_1(double[, ] ss)
        {
            double[][] matrix = new double[ss.GetLength(0)][];

            for (int i = 0; i < ss.GetLength(0); i++)
            {
                matrix[i] = new double[ss.GetLength(1)];
            }



            for (int i = 0; i < ss.GetLength(0); i++)
            {
                for (int j = 0; j < ss.GetLength(1); j++)
                {
                    matrix[i][j] = ss[i,j];
                }
            }
            return matrix;
        }


     /*   public void Math_Mul(Matrix a)
        {
            double[,] vC = new double[a.ColCount, this.RowCount];
            externExtensions.MatMul(vC, a._values, this._values, a.RowCount, this.ColCount, a.ColCount);
            _values = vC;
        }*/

        public override Matrix Multiply(Matrix matA, Matrix matB)
        {
            var result = new Matrix(matA.ColCount, matB.RowCount, Matrix.CreationVariant.ZeroFilled);
            double[,] vC = new double[matA.ColCount, matB.RowCount];
            //var ss = Stopwatch.StartNew();
         //   double[,] a = Transformer1_2(matA._values);
            //double[,] b = Transformer1_2(matB._values);
          //  ss.Stop();
          //  Console.WriteLine(ss.Elapsed.TotalMilliseconds);
            externExtensions.MatMul(vC, Transformer1_2(matA._values), Transformer1_2(matB._values), matA.RowCount, matB.ColCount, matA.ColCount);
          //  ss.Start();
            result._values = Transformer2_1(vC);
           // ss.Stop();
          //  Console.WriteLine(ss.Elapsed.TotalMilliseconds);

            return result;
        }
    }
}
