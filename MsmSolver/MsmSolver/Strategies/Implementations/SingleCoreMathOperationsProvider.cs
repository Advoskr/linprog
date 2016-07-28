using System;

namespace MsmSolver.Strategies
{
    public class SingleCoreMathOperationsProvider : IMathOperationsProvider
    {
        public virtual Matrix Multiply(Matrix a, Matrix b)
        {
            if (a.ColCount == b.RowCount)
            {
                double[][] val = new double[a.RowCount][];
                for (int i = 0; i < a.RowCount; i++)
                {
                    val[i] = new double[b.ColCount];
                }

                for (int i = 0; i < a.RowCount; i++)
                {
                    for (int j = 0; j < b.ColCount; j++)
                    {
                        for (int k = 0; k < a.ColCount; k++)
                        {
                            val[i][j] += a.Values[i][k] * b.Values[k][j];
                        }
                    }
                }
                Matrix res = new Matrix(val);
                return res;
            }
            else
                throw new Exception("Wrong matrix dimensions");
        }

        public Matrix Divide(Matrix left, Matrix right)
        {
            throw new System.NotImplementedException();
        }

        public Matrix Sum(Matrix left, Matrix right)
        {
            throw new System.NotImplementedException();
        }

        public double Multiply(Vector a, Vector b)
        {
            if (a.Dimension != b.Dimension)
                throw new Exception("vectors dimensions must be similar");
            double results = 0;
            for (int i = 0; i < a.Dimension; i++)
            {
                results += a[i] * b[i];
            }
            return results;
        }

        public Vector Sum(Vector left, Vector right)
        {
            throw new System.NotImplementedException();
        }

        public Vector Subtract(Vector left, Vector right)
        {
            throw new System.NotImplementedException();
        }

        public Vector Divide(Vector left, Vector right)
        {
            throw new System.NotImplementedException();
        }

        public Vector Multiply(Vector left, Matrix right)
        {
            double[] res = new double[right.ColCount];
            for (int i = 0; i < right.ColCount; i++)
            {
                for (int j = 0; j < left.Dimension; j++)
                {
                    res[i] += left[j] * right.Values[j][i];
                }
            }
            return new Vector(res);
        }

        public Vector Multiply(Matrix left, Vector right)
        {
            double[] res = new double[left.RowCount];
            for (int i = 0; i < left.RowCount; i++)
            {
                for (int j = 0; j < right.Dimension; j++)
                {
                    res[i] += left[i, j] * right[j];
                }
            }
            return new Vector(res);
        }
    }
}