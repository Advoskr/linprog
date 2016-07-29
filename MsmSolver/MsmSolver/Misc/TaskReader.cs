using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsmSolver.Misc
{
    public class TaskReader
    {
        public Task ReadFromSmallFile(StreamReader streamReader)
        {
            int kolOgran = 0, kolPerem = 0;
            int ogranNumber = 0;

            var result = new Task();

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
            for (int i = 0; i < kolOgran; i++)
            {
                Avals[i] = new double[kolPerem];
            }
            result.C = new Vector(kolPerem);
            result.A0 = new Vector(kolOgran);
            result.Signs = new Signs[kolOgran];
            result.Direction = new Direction();

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
                        result.Direction = Direction.Max;
                    else
                        result.Direction = Direction.Min;
                }

                if (isReadingOgrans)
                {
                    if (line.Contains("<="))
                    {
                        result.Signs[ogranNumber] = Signs.M_R;
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
                        result.Signs[ogranNumber] = Signs.B_R;
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
                        result.Signs[ogranNumber] = Signs.R;
                        result.A0[ogranNumber] = Convert.ToDouble(line.Remove(0, line.IndexOf("=") + 1));
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
