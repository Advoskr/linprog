using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MsmSolver
{
    public enum Signs
    {
        M_R, R, B_R
    }

    public enum Direction
    {
        Max, Min
    }
    class Simplex_Task
    {
        public Vector C { get; protected set; }

        public Matrix A { get; protected set; }

        public Vector A0 { get; protected set; }

        public Signs[] signs { get; protected set; }

        public Direction direction { get; protected set; }

        public Simplex_Task()
        {

        }

        public Simplex_Task(Vector c, Matrix a, Vector a0, Signs[] s, Direction dir)
        {
            this.C = c;
            this.A = a;
            this.A0 = a0;
            this.signs = s;
            this.direction = dir;
        }

        public Simplex_Task ReadFromSmallFile(StreamReader streamReader)
        {
            int kolOgran = 0, kolPerem = 0;
            int ogranNumber = 0;

            var result = new Simplex_Task();

            bool isReadingOgrans = false;

            String[] vals;

            string line = streamReader.ReadLine();

            while (line != null)
            {
                if (line.ToUpper() == "$OGRAN") kolOgran = Convert.ToInt32(streamReader.ReadLine().Replace(" ", ""));
                if (line.ToUpper() == "$PEREM") kolPerem = Convert.ToInt32(streamReader.ReadLine().Replace(" ", ""));

                line = streamReader.ReadLine();

                if (kolOgran > 0 && kolPerem > 0) break;
            }

            double[][] Avals = new double[kolOgran][];
            for(int i = 0; i < kolOgran; i++)
            {
                Avals[i] = new double[kolPerem];
            }
            result.C = new Vector(kolPerem);
            result.A0 = new Vector(kolOgran);
            result.signs = new Signs[kolOgran];
            result.direction = new Direction();

            while (line != null)
            {
                if (line.ToUpper() == "FUNCTION")
                {
                    vals = streamReader.ReadLine().Split(' ');
                    for (int i = 0; i < kolPerem; i++)
                    {
                        result.C[i] = Convert.ToDouble(vals[i]);
                    }
                }

                if (line.ToUpper() == "$CF")
                {
                    if (streamReader.ReadLine().ToUpper() == "MAX")
                        result.direction = Direction.Max;
                    else
                        result.direction = Direction.Min;
                }

                if (isReadingOgrans)
                {
                    if (line.Contains("<="))
                    {
                        result.signs[ogranNumber] = Signs.M_R;
                        result.A0[ogranNumber] = Convert.ToDouble(line.Remove(0, line.IndexOf("<=") + 2));
                        line = line.Remove(line.IndexOf("<="), line.Length - line.IndexOf("<="));
                        vals = line.Split(' ');
                        for (int i = 0; i < kolPerem; i++)
                        {
                            Avals[ogranNumber][i] = Convert.ToDouble(vals[i]);
                        }
                        ogranNumber++;
                    }

                    if (line.Contains(">="))
                    {
                        result.signs[ogranNumber] = Signs.B_R;
                        result.A0[ogranNumber] = Convert.ToDouble(line.Remove(0, line.IndexOf(">=") + 2));
                        line = line.Remove(line.IndexOf(">="), line.Length - line.IndexOf(">="));
                        vals = line.Split(' ');
                        for (int i = 0; i < kolPerem; i++)
                        {
                            Avals[ogranNumber][i] = Convert.ToDouble(vals[i]);
                        }
                        ogranNumber++;
                    }

                    if (line.Contains("="))
                    {
                        result.signs[ogranNumber] = Signs.R;
                        result.A0[ogranNumber] = Convert.ToDouble(line.Remove(0, line.IndexOf("=") + 2));
                        line = line.Remove(line.IndexOf("="), line.Length - line.IndexOf("="));
                        vals = line.Split(' ');
                        for (int i = 0; i < kolPerem; i++)
                        {
                            Avals[ogranNumber][i] = Convert.ToDouble(vals[i]);
                        }
                        ogranNumber++;
                    }

                }

                if (line.ToUpper() == "OGRAN") isReadingOgrans = true;
                line = streamReader.ReadLine();
            }
            result.A = new Matrix(Avals);


            return result;
        }
    }
}
