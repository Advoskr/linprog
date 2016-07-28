using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MsmSolver
{

    public static class externExtensions
    {
        #region C++Features
        [DllImport("C++Extensions.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void MatMul(double[,] vC, double[,] vA,
         double[,] vB, int M, int N, int W);

        [DllImport("C++Extensions.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void MatMulFire(double[,] vC, double[,] vA,
         double[,] vB, int M, int N, int W);

        [DllImport("C++Extensions.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void TilingMultiplication(double[,] vA,
         double[,] vB, double[,] vC, int M, int N, int W, int TS);

        [DllImport("C++Extensions.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void MatVectMul(double[] vC, double[,] vA,
         double[] vB, int M, int W);

        //[DllImport("C++Extensions.dll", CallingConvention = CallingConvention.Cdecl)]
        //public extern static void MatMulCPU(double[,] vC, double[,] vA,
        // double[,] vB, int M, int N, int W);

        //не работает, допилить!
        [DllImport("C++Extensions.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string InitializeLibrary();

        [DllImport("C++Extensions.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern string ShowAccelerator();
        #endregion

        public static void Initialize()
        {
            double[,] aDoubles = new double[2,2] { { 1, 1 }, { 1, 1 } };
            double[,] bDoubles = new double[2, 2] { { 1, 1 }, { 1, 1 } };
            double[,] cDoubles = new double[2, 2] { { 1, 1 }, { 1, 1 } };
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //Console.WriteLine("Default Accelerator:");
            //ShowAccelerator();
            MatMul(aDoubles, bDoubles, cDoubles, 2, 2, 2);
            sw.Stop();
            Console.WriteLine("Extensions Initialized in {0} milliseconds", sw.Elapsed.TotalMilliseconds);

        }
    }
}