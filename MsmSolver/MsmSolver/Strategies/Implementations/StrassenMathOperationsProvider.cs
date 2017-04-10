using System;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace MsmSolver.Strategies.Implementations
{
    public class StrassenMathOperationsProvider : SingleCoreMathOperationsProvider
    {
        public override Matrix Multiply(Matrix a, Matrix b)
        {
            return StrassenMultiply(a, b);
        }

        private const int MinMatrixSize = 128;

        public static Matrix ZeroMatrix(int iRows, int iCols)       // Function generates the zero matrix
        {
            Matrix matrix = new Matrix(iRows, iCols);
            //for (int i = 0; i < iRows; i++)
            //    for (int j = 0; j < iCols; j++)
            //        matrix[i, j] = 0;
            return matrix;
        }

        private static void SafeAplusBintoC(Matrix A, int xa, int ya, Matrix B, int xb, int yb, Matrix C, int size)
        {
            for (int i = 0; i < size; i++)          // rows
                for (int j = 0; j < size; j++)     // cols
                {
                    C[i, j] = 0;
                    if (xa + j < A.ColCount && ya + i < A.RowCount) C[i, j] += A[ya + i, xa + j];
                    if (xb + j < B.ColCount && yb + i < B.RowCount) C[i, j] += B[yb + i, xb + j];
                }
        }

        private static void SafeAminusBintoC(Matrix A, int xa, int ya, Matrix B, int xb, int yb, Matrix C, int size)
        {
            for (int i = 0; i < size; i++)          // rows
                for (int j = 0; j < size; j++)     // cols
                {
                    C[i, j] = 0;
                    if (xa + j < A.ColCount && ya + i < A.RowCount) C[i, j] += A[ya + i, xa + j];
                    if (xb + j < B.ColCount && yb + i < B.RowCount) C[i, j] -= B[yb + i, xb + j];
                }
        }

        private static void SafeACopytoC(Matrix A, int xa, int ya, Matrix C, int size)
        {
            for (int i = 0; i < size; i++)          // rows
                for (int j = 0; j < size; j++)     // cols
                {
                    C[i, j] = 0;
                    if (xa + j < A.ColCount && ya + i < A.RowCount) C[i, j] += A[ya + i, xa + j];
                }
        }

        private static void AplusBintoC(Matrix A, int xa, int ya, Matrix B, int xb, int yb, Matrix C, int size)
        {
            for (int i = 0; i < size; i++)          // rows
                for (int j = 0; j < size; j++) C[i, j] = A[ya + i, xa + j] + B[yb + i, xb + j];
        }

        private static void AminusBintoC(Matrix A, int xa, int ya, Matrix B, int xb, int yb, Matrix C, int size)
        {
            for (int i = 0; i < size; i++)          // rows
                for (int j = 0; j < size; j++) C[i, j] = A[ya + i, xa + j] - B[yb + i, xb + j];
        }

        private static void ACopytoC(Matrix A, int xa, int ya, Matrix C, int size)
        {
            for (int i = 0; i < size; i++)          // rows
                for (int j = 0; j < size; j++) C[i, j] = A[ya + i, xa + j];
        }
        private static Matrix StrassenMultiply(Matrix A, Matrix B)                // Smart matrix multiplication
        {
            if (A.ColCount != B.RowCount) throw new Exception("Wrong dimension of matrix!");

            Matrix R;

            int msize = Math.Max(Math.Max(A.RowCount, A.ColCount), Math.Max(B.RowCount, B.ColCount));

            if (msize < MinMatrixSize)
            {
                R = ZeroMatrix(A.RowCount, B.ColCount);
                //Parallel.For(0, R.RowCount, i =>
                //{
                for (int i = 0; i < R.RowCount; i++)
                    for (int j = 0; j < R.ColCount; j++)
                        for (int k = 0; k < A.ColCount; k++)
                            R[i, j] += A[i, k]*B[k, j];
                //});
                return R;
            }

            int size = 1; int n = 0;
            while (msize > size) { size *= 2; n++; };
            int h = size / 2;


            Matrix[,] mField = new Matrix[n, 9];

            /*
             *  8x8, 8x8, 8x8, ...
             *  4x4, 4x4, 4x4, ...
             *  2x2, 2x2, 2x2, ...
             *  . . .
             */

            for (int i = 0; i < n - 4; i++)          // RowCount
            {
                var z = (int)Math.Pow(2, n - i - 1);
                for (int j = 0; j < 9; j++) mField[i, j] = new Matrix(z, z);
            }

            var tasks = new System.Threading.Tasks.Task[7];
            //tasks[0] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                SafeAplusBintoC(A, 0, 0, A, h, h, mField[0, 0], h);
                SafeAplusBintoC(B, 0, 0, B, h, h, mField[0, 1], h);
                StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 1], 1, mField); // (A11 + A22) * (B11 + B22);
            //});
            //tasks[1] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                SafeAplusBintoC(A, 0, h, A, h, h, mField[0, 0], h);
                SafeACopytoC(B, 0, 0, mField[0, 1], h);
                StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 2], 1, mField); // (A21 + A22) * B11;
            //});
            //tasks[2] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                SafeACopytoC(A, 0, 0, mField[0, 0], h);
                SafeAminusBintoC(B, h, 0, B, h, h, mField[0, 1], h);
                StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 3], 1, mField); //A11 * (B12 - B22);
            //});
            //tasks[3] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                SafeACopytoC(A, h, h, mField[0, 0], h);
                SafeAminusBintoC(B, 0, h, B, 0, 0, mField[0, 1], h);
                StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 4], 1, mField); //A22 * (B21 - B11);
            //});
            //tasks[4] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                SafeAplusBintoC(A, 0, 0, A, h, 0, mField[0, 0], h);
                SafeACopytoC(B, h, h, mField[0, 1], h);
                StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 5], 1, mField); //(A11 + A12) * B22;
            //});
            //tasks[5] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                SafeAminusBintoC(A, 0, h, A, 0, 0, mField[0, 0], h);
                SafeAplusBintoC(B, 0, 0, B, h, 0, mField[0, 1], h);
                StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 6], 1, mField); //(A21 - A11) * (B11 + B12);
            //});
            //tasks[6] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                SafeAminusBintoC(A, h, 0, A, h, h, mField[0, 0], h);
                SafeAplusBintoC(B, 0, h, B, h, h, mField[0, 1], h);
                StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 7], 1, mField); // (A12 - A22) * (B21 + B22);
            //});

            //System.Threading.Tasks.Task.WaitAll(tasks);
            R = new Matrix(A.RowCount, B.ColCount);                  // result

            var tasks2 = new System.Threading.Tasks.Task[4];
            /// C11

            tasks2[0] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < Math.Min(h, R.RowCount); i++)          // RowCount
                    for (int j = 0; j < Math.Min(h, R.ColCount); j++)     // ColCount
                        R[i, j] = mField[0, 1 + 1][i, j] + mField[0, 1 + 4][i, j] - mField[0, 1 + 5][i, j] + mField[0, 1 + 7][i, j];
            });
            /// C12

            tasks2[1] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < Math.Min(h, R.RowCount); i++)          // RowCount
                    for (int j = h; j < Math.Min(2 * h, R.ColCount); j++)     // ColCount
                        R[i, j] = mField[0, 1 + 3][i, j - h] + mField[0, 1 + 5][i, j - h];
            });
            /// C21

            tasks2[2] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                for (int i = h; i < Math.Min(2 * h, R.RowCount); i++)          // RowCount
                    for (int j = 0; j < Math.Min(h, R.ColCount); j++)     // ColCount
                        R[i, j] = mField[0, 1 + 2][i - h, j] + mField[0, 1 + 4][i - h, j];
            });
            /// C22

            tasks2[3] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                for (int i = h; i < Math.Min(2 * h, R.RowCount); i++) // RowCount
                    for (int j = h; j < Math.Min(2 * h, R.ColCount); j++) // ColCount
                        R[i, j] = mField[0, 1 + 1][i - h, j - h] - mField[0, 1 + 2][i - h, j - h] +
                              mField[0, 1 + 3][i - h, j - h] + mField[0, 1 + 6][i - h, j - h];
            });
            System.Threading.Tasks.Task.WaitAll(tasks2);
            return R;
        }

        // function for square matrix 2^N x 2^N

        private static void StrassenMultiplyRun(Matrix A, Matrix B, Matrix C, int l, Matrix[,] f)    // A * B into C, level of recursion, matrix field
        {
            int size = A.RowCount;
            int h = size / 2;

            if (size < MinMatrixSize)
            {
                Parallel.For(0, C.RowCount, i =>
                {
                    //for (int i = 0; i < C.RowCount; i++)
                        for (int j = 0; j < C.ColCount; j++)
                        {
                            C[i, j] = 0;
                            for (int k = 0; k < A.ColCount; k++) C[i, j] += A[i, k]*B[k, j];
                        }
                });
                return;
            }

            var tasks = new System.Threading.Tasks.Task[7];
            //tasks[0] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                AplusBintoC(A, 0, 0, A, h, h, f[l, 0], h);
                AplusBintoC(B, 0, 0, B, h, h, f[l, 1], h);
                StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 1], l + 1, f); // (A11 + A22) * (B11 + B22);
            //});
            //tasks[1] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                AplusBintoC(A, 0, h, A, h, h, f[l, 0], h);
                ACopytoC(B, 0, 0, f[l, 1], h);
                StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 2], l + 1, f); // (A21 + A22) * B11;
            //});
            //tasks[2] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                ACopytoC(A, 0, 0, f[l, 0], h);
                AminusBintoC(B, h, 0, B, h, h, f[l, 1], h);
                StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 3], l + 1, f); //A11 * (B12 - B22);
            //});
            //tasks[3] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                ACopytoC(A, h, h, f[l, 0], h);
                AminusBintoC(B, 0, h, B, 0, 0, f[l, 1], h);
                StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 4], l + 1, f); //A22 * (B21 - B11);
            //});
            //tasks[4] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                AplusBintoC(A, 0, 0, A, h, 0, f[l, 0], h);
                ACopytoC(B, h, h, f[l, 1], h);
                StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 5], l + 1, f); //(A11 + A12) * B22;
            //});
            //tasks[5] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                AminusBintoC(A, 0, h, A, 0, 0, f[l, 0], h);
                AplusBintoC(B, 0, 0, B, h, 0, f[l, 1], h);
                StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 6], l + 1, f); //(A21 - A11) * (B11 + B12);
            //});
            //tasks[6] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                AminusBintoC(A, h, 0, A, h, h, f[l, 0], h);
                AplusBintoC(B, 0, h, B, h, h, f[l, 1], h);
                StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 7], l + 1, f); // (A12 - A22) * (B21 + B22);
            //});
            //System.Threading.Tasks.Task.WaitAll(tasks);
            
            var tasks2 = new System.Threading.Tasks.Task[4];

            /// C11
            //tasks2[0] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            for (int i = 0; i < h; i++) // RowCount
                    for (int j = 0; j < h; j++) // ColCount
                        C[i, j] = f[l, 1 + 1][i, j] + f[l, 1 + 4][i, j] - f[l, 1 + 5][i, j] + f[l, 1 + 7][i, j];
            //});

            /// C12
            //tasks2[1] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            for (int i = 0; i < h; i++)          // RowCount
                    for (int j = h; j < size; j++)     // ColCount
                        C[i, j] = f[l, 1 + 3][i, j - h] + f[l, 1 + 5][i, j - h];
            //});
            /// C21
            //tasks2[2] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            for (int i = h; i < size; i++)          // RowCount
                    for (int j = 0; j < h; j++)     // ColCount
                        C[i, j] = f[l, 1 + 2][i - h, j] + f[l, 1 + 4][i - h, j];
            //});
            /// C22
            //tasks2[3] = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            for (int i = h; i < size; i++)          // RowCount
                    for (int j = h; j < size; j++)     // ColCount
                        C[i, j] = f[l, 1 + 1][i - h, j - h] - f[l, 1 + 2][i - h, j - h] + f[l, 1 + 3][i - h, j - h] + f[l, 1 + 6][i - h, j - h];
            //});
            //System.Threading.Tasks.Task.WaitAll(tasks2);
        }
    }
}