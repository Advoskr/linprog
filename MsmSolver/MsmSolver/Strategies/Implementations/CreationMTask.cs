using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsmSolver.Strategies
{
   public class CreationMTask
    {
        public Task MTask(Task task)
        {
            var result = new Task();
            Basis basis = new Basis(); // локальный базис, нужен для определения базисных векторов в задаче в каноническом виде
            basis.Values = new Matrix(task.A.RowCount, task.A.RowCount, Matrix.CreationVariant.ZeroFilled);
            basis.VectorIndexes = new int[basis.Values.ColCount];

           
            result.A0 = new Vector(task.A0.Dimension);
            
            result.Direction = new Direction();
            result.Signs = new Signs[task.A.RowCount];

            bool inBasis = false; // есть ли такое вектор в базисе
            int counter = 0;      // Кол-во строк, куда нужно добавлять новую переменную
            int indexesIdx = 0;

            bool[] ImportantRows = new bool[task.A.RowCount]; // массив булов, true - в строке есть единица из базисного столбца
            for (int j = 0; j < task.A.ColCount; j++)  // после прохождения цикла массив булов, показывающий, где нужно добавить переменную индексы соответствующие false)
            {
                int Num_Row = 0;
                for (int i = 0; i < task.A.RowCount; i++)
                {
                    if (task.A[i][j] != 1 && task.A[i][j] != 0)
                    { inBasis = false; break; }
                    if (task.A[i][j] == 1)
                    {
                        //we already had basis value here
                        if (inBasis)
                        { inBasis = false; break; }
                        //this row wasn't basis one, but now it is. 
                        Num_Row = i;
                        inBasis = true;
                    }
                }
                if (!inBasis)
                    continue;


                for (int i = 0; i < task.A.ColCount; i++)
                {
                    if (basis.VectorIndexes.Contains(i) && task.A[Num_Row][i] == task.A[Num_Row][j] && Proverka(basis.VectorIndexes))
                        { inBasis = false; }
                }
                if (!inBasis)   // если есть такой же вектор, переходим на другой столбец с помощью continue
                    continue;

                if (inBasis)
                {
                    //A has an identity vector in it, so we can copy it to out bobr. It'll guarantee that 1 is in correct position.
                    basis.Values.ChangeColumn(indexesIdx, task.A.GetColumn(j));
                    //vector index is our column index.
                    basis.VectorIndexes[indexesIdx++] = j;
                    // set the flag back again
                    inBasis = false;
                    ImportantRows[Num_Row] = true;
                }
            }

            for (int i = 0; i < ImportantRows.Length; i++) // сколько добавить переменных, важно для Аvals          
            {
                if (ImportantRows[i] == false)
                counter++;
            }

            var Avals = new double[task.A.RowCount][];// Будущая матрица result.A;
            for (int i = 0; i < task.A.RowCount;i++)
            {
                Avals[i] = new double[task.A.ColCount + counter];
            }

            result.C = new Vector(task.A.ColCount + counter);

            int Imp_counter = 0; // Считает, какой по счёту False, чтобы знать номер столбца, который вводить номер столбца = task.A.ColCount - 1 + Imp_counter

            for (int i = 0 ; i < task.A.RowCount; i++)
            {
                if (ImportantRows[i] == false)
                    Imp_counter++;

                result.Signs[i] = Signs.R;
                for (int j = 0; j < task.A.ColCount + counter; j++)
                {
                    if (ImportantRows[i] == true && j < task.A.ColCount)
                        Avals[i][j] = task.A[i][j];
                    if (ImportantRows[i] == true && j >= task.A.ColCount)
                        Avals[i][j] = 0;
                    if (ImportantRows[i] == false && j < task.A.ColCount)
                        Avals[i][j] = task.A[i][j];
                    if (ImportantRows[i] == false && j >=task.A.ColCount)
                    {
                        if (j == task.A.ColCount - 1 + Imp_counter)
                            Avals[i][j] = 1;
                        else
                            Avals[i][j] = 0;
                    }

                }
            }

            result.A = new Matrix(Avals);
            result.Direction = Direction.Max;
            result.A0 = task.A0;

            for (int i = 0; i < task.A.ColCount + counter; i++)
            {
                if (i < task.A.ColCount)
                    result.C[i] = task.C[i];
                else
                    result.C[i] = -int.MaxValue / 4;
            }

            return result;
        }

        public static bool Proverka(int[] Jenia) // проверка на кол-во нулевый элементов массива basis.VectorIndexes
        {
            bool TestPerem = false;

            int counter = 0; // Кол-во нулей в массиве

            for (int i = 0; i < Jenia.Length; i++)
            {
                if (Jenia[i] == 0)
                    counter++;
            }

            if (counter > 1)
                TestPerem = false;
            else
                TestPerem = true;

            return TestPerem;

        }

     /*   public Basis GetInitialBasis(Task task)
        {
            Basis basis = new Basis();
            basis.Values = new Matrix(task.A.RowCount, task.A.RowCount, Matrix.CreationVariant.IdentityMatrix);
            basis.VectorIndexes = new int[basis.Values.ColCount];

            bool isBasis = false;
            int indexesIdx = 0;
            for (int j = 0; j < task.A.ColCount; j++)
            {
                int k = 0;
                for (int i = 0; i < task.A.RowCount; i++)
                {
                    // not identity matrix, so this cannot be canonical basis.
                    if (task.A[i][j] != 1 && task.A[i][j] != 0) { isBasis = false; break; }
                    //found basis value. all other values must be zero
                    if (task.A[i][j] == 1)
                    {
                        //we already had basis value here
                        if (isBasis)
                        { isBasis = false; break; }
                        //this row wasn't basis one, but now it is. 
                        k = i;
                        isBasis = true;
                    }
                }
                //checked row. Now, if it's basis, we go to this block.
                for (int i = 0; i < task.A.ColCount; i++)
                {
                    if (basis.VectorIndexes.Contains(i) && task.A[k][i] == task.A[k][j])
                    { isBasis = false; break; }
                }

                if (isBasis)
                {
                    //A has an identity vector in it, so we can copy it to out bobr. It'll guarantee that 1 is in correct position.
                    basis.Values.ChangeColumn(indexesIdx, task.A.GetColumn(j));
                    //vector index is our column index.
                    basis.VectorIndexes[indexesIdx++] = j;
                    // set the flag back again
                    isBasis = false;
                }
                //else, we continue to check A matrix. 

            }
            if (indexesIdx != basis.VectorIndexes.Length)
                throw new Exception("Basis wasn't found correctly");
            return basis;
        }*/




    }
}
