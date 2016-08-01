using System;
using System.Threading.Tasks;

namespace MsmSolver
{
    public class Matrix
    {
        public enum CreationVariant
        {
            ZeroFilled,
            IdentityMatrix,
            OneValuesFilled,
        }

        public int ColCount { get; set; }

        public int RowCount { get; set; }

        //Important! Jagged arrays seem to be faster than multidim arrays because of CPU cache usage. 
        //See here:http://stackoverflow.com/questions/3229442/why-is-matrix-multiplication-in-net-so-slow
        public double[][] Values
        {
            get { return _values; }
            set { _values = value; }
        }

        public double[][] _values;

        public double this[int index1, int index2]
        {
            get { return _values[index1][index2]; }
            set { _values[index1][index2] = value; }
        }

        //public Matrix RightJoin (Matrix a)
        //{
        //    ColCount = +a.ColCount;
        //    _values
        //    return this;
        //}

        public Vector this[int index]
        {
            get
            {
                if (index < 0 || index >= _values.Length) return new Vector();
                else
                {
                    double[] vals = new double[ColCount];
                    for (int i = 0; i < ColCount; i++)
                    {
                        vals[i] = _values[index][i];
                    }
                    return new Vector(vals);
                }

            }

        }

        public Matrix(int Rows, int Cols, CreationVariant matrixType)
        {
            _values = new double[Rows][];
            for (int i = 0; i < Rows; i++)
            {
                _values[i] = new double[Cols];
            }
            RowCount = Rows;
            ColCount = Cols;
            switch (matrixType)
            {
                case CreationVariant.IdentityMatrix:
                    if (Rows != Cols) throw new Exception("Matrix dimensions are not similar");
                    for (int i = 0; i < Rows; i++)
                    {
                        _values[i][i] = 1;
                    }
                    break;
                case CreationVariant.ZeroFilled:
                    //:)
                    break;
            }
        }

        public Matrix(double[][] val)
        {
            Initialize(val);
        }

        public Matrix()
        {
        }

        public void Initialize(double[][] val)
        {
            RowCount = val.GetLength(0);
            ColCount = val[0].GetLength(0);
            _values = val;
            //_values = new double[RowCount, ColCount];
          //  Buffer.BlockCopy(val, 0, _values, 0, _values.Length*sizeof(double));
            //Array.Copy(val, 0, _values, 0, _values.Length);
        }

        public Vector GetColumn(int index)
        {
            double[] res = new double[RowCount];
            for (int i = 0; i < RowCount; i++)
            {
                res[i] = _values[i][index];
            }
            return new Vector(res);
        }

        public void ChangeColumn(int index, Vector values)
        {
            for (int i = 0; i < RowCount; i++)
            {
                _values[i][index] = values[i];
            }
        }

        //public void Math_Mul(Matrix a)
        //{
        //    double[,] vC = new double[a.ColCount, this.RowCount];
        //    externExtensions.MatMul(vC, a._values, this._values, a.RowCount, this.ColCount, a.ColCount);
        //    _values = vC;
        //}

        //public void Math_Mul_Tile(Matrix a)
        //{
        //    double[,] vC = new double[a.ColCount, this.RowCount];
        //    externExtensions.MatMulFire(vC, a._values, this._values, a.RowCount, this.ColCount, a.ColCount);
        //    _values = vC;
        //}

    /*    public static Matrix MultiplyMatricesParallel(Matrix matA, Matrix matB)
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
        }*/

        //интересно, нам на "направление" вектора срать?)))
    }
}