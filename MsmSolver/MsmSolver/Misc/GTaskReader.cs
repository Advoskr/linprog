using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MsmSolver.Misc
{
    public class GTaskReader
    {
        
        public Task ReadFromGFile(StreamReader streamReader)
        {
            var result = new Task();

            int colCount = 0; // кол-во переменных
            int colOgrans = 0;// кол-во ограничений

            List<string> perem = new List<string>();

            String line = streamReader.ReadLine();

            while(!line.Contains("Ext"))
            {
                line = streamReader.ReadLine();
            }

            if (line.Remove(0, line.IndexOf("=") + 2) == "Max")
            {
                result.Direction = Direction.Max;                   // Заполнили направление функции
            }
            else
                result.Direction = Direction.Min;

            while (line != "[Variables]")
            {
                line = streamReader.ReadLine();
            }

            line = streamReader.ReadLine();

            while (line != "[Goal]")
            {
                colCount++;
                perem.Add(line.Substring(0, line.IndexOf(":")));    // имеется массив строк с названиями переменных
                line = streamReader.ReadLine();
            }

            result.C = new Vector(colCount);

            line = streamReader.ReadLine(); // строка с ЦФ

            for (int i = 0; i < colCount; i++)  // Заполнение коэффициентов ЦФ
            {
                if (line.IndexOf(perem[i]) == -1)
                {
                    result.C[i] = 0;
                }

                else
                {
                    if (line.IndexOf(perem[i]) - 1 == -1)
                        result.C[i] = 1;
                    else
                    if (line[line.IndexOf(perem[i]) - 1] == '-')
                        result.C[i] = -1;
                    else
                    if (line[line.IndexOf(perem[i]) - 1] == '+')
                        result.C[i] = 1;
                    else
                    if (line[line.IndexOf(perem[i]) - 1] == '*')
                    {
                        int j = 1;
                        string internalLine = "";
                        while (line[line.IndexOf(perem[i]) - 1 - j] != '-' || line[line.IndexOf(perem[i]) - 1 - j] != '+')
                        {
                            internalLine = line[line.IndexOf(perem[i]) - 1 - j] + internalLine;
                            j++;
                        }

                        internalLine = line[line.IndexOf(perem[i]) - 1 - j] + internalLine;
                        result.C[i] = Convert.ToDouble(internalLine);

                    }

                }
            }

            line = streamReader.ReadLine();
            line = streamReader.ReadLine(); // Попали на первое ограничение

            List<double> signs = new List<double>();
            List<double> a0 = new List<double>();

            List<double[]> a = new List<double[]>();

            while(line != null) // Заполнение А А0 и Signs
            {
                double[] internalDouble = new double[colCount];
               

                for (int i = 0; i < colCount; i++) // заполнение строки будущей матрицы A
                {
                    if (line.IndexOf(perem[i]) == -1)
                        internalDouble[i] = 0;
                    else
                    {
                        if (line.IndexOf(perem[i]) - 1 == -1)
                            internalDouble[i] = 1;
                        else
                        if (line[line.IndexOf(perem[i]) - 1] == '-')
                            internalDouble[i] = -1;
                        else
                        if (line[line.IndexOf(perem[i]) - 1] == '+')
                            internalDouble[i] = 1;
                        else
                        if (line[line.IndexOf(perem[i]) - 1] == '*')
                        {
                            int j = 1;
                            string internalLine = "";
                            while ((line[line.IndexOf(perem[i]) - 1 - j] != '-') && (line[line.IndexOf(perem[i]) - 1 - j] != '+'))
                            {
                                internalLine = line[line.IndexOf(perem[i]) - 1 - j] + internalLine;
                                j++;
                            }
                            if(line[line.IndexOf(perem[i]) - 1 - j] == '-')
                            { 
                                internalLine = line[line.IndexOf(perem[i]) - 1 - j] + internalLine;
                            }

                            internalLine = internalLine.Replace(".", ",");

                            internalDouble[i] = Convert.ToDouble(internalLine);

                        }
                    }

                  }

                if (line.Contains("<="))
                {
                    signs.Add(0);
                    line = line.Remove(0, line.IndexOf("<=") + 2);
                    line = line.Replace(".", ",");
                    a0.Add(Convert.ToDouble(line));

                    //result.A0[ogranNumber] = Convert.ToDouble(line.Remove(0, line.IndexOf("<=") + 2));
                }
                else
                if (line.Contains(">="))
                {
                    signs.Add(2);
                    line = line.Remove(0, line.IndexOf(">=") + 2);
                    line = line.Replace(".", ",");
                    a0.Add(Convert.ToDouble(line));
                }
                else
                if (line.Contains("="))
                {
                    signs.Add(1);
                    line = line.Remove(0, line.IndexOf("=") + 1);
                    line = line.Replace(".", ",");
                    a0.Add(Convert.ToDouble(line));
                }


                colOgrans++;
                a.Add(internalDouble);
                line = streamReader.ReadLine();
            }

            result.A0 = new Vector(colOgrans);
            result.A = new Matrix(colOgrans, colCount);
            result.Signs = new Signs[colOgrans];

            for (int i = 0; i < colOgrans; i++)
            {
                result.A0[i] = a0[i];
                result.Signs[i] = (Signs)signs[i];
                for (int j = 0; j < colCount; j++)
                {
                    result.A._values[i][j] = a[i][j];
                }

             }

            return result;
        }

    }
}

