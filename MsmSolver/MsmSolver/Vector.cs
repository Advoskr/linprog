using System;
using System.Threading.Tasks;

namespace MsmSolver
{
    public class Vector
    {
        public int Dimension;

        public double[] Value { get; set; }

        public double this[int index]
        {
            get
            {
                return (index < 0 || index >= Value.Length) ? 0 : Value[index];
            }

            set { if (!(index < 0 || index >= Value.Length)) Value[index] = value; }
        }

        public Vector()
        {
            Dimension = 0;
            Value = new double[0];
        }

        public Vector(int _dimension)
        {
            Dimension = _dimension;
            Value = new double[_dimension];
        }

        public Vector(double[] values)
        {
            Dimension = values.Length;
            Value = values;
        }

        public void Initialize(double[] a)
        {
            Dimension = a.Length;
            Array.Copy(a, Value, Dimension);
        }

        public static Vector operator *(Vector a, double value)
        {
            double[] values = new double[a.Dimension];

            for (int i = 0; i < a.Dimension; i++)
            {
                values[i] = a[i] * value;
            }
            return new Vector(values);

        }

        public string AsString()
        {
            string res = "";
            for (int i = 0; i < Dimension; i++)
            {
                res += "X" + i.ToString() + "=" + Value[i].ToString() + ";";
            }
            return res;
        }

        public static double operator *(Vector a, Vector b)
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

        public static double operator &(Vector a, Vector b)
        {
            if (a.Dimension != b.Dimension)
                throw new Exception("vectors dimensions must be similar");
            double results = 0;
            object syncObj = new object();
            Parallel.For(0, a.Dimension, i =>
            {
                double temp = a[i] * b[i];
                lock (syncObj)
                {
                    results += temp;
                }
            });
            return results;
        }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < Dimension; i++)
            {
                result += " " + Value[i];
            }
            return result;
        }
    }
}