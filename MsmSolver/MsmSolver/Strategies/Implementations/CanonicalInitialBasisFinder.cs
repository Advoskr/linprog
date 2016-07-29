using System;
using System.Linq;

namespace MsmSolver.Strategies
{
    public class CanonicalInitialBasisFinder : IInitialBasisFinder
    {
        public Basis GetInitialBasis(Task task)
        {
            Basis basis = new Basis();
            basis.Values = new Matrix(task.A.RowCount, task.A.RowCount, Matrix.CreationVariant.IdentityMatrix);
            basis.VectorIndexes = new int[basis.Values.ColCount];

            bool isBasis = false;
            int indexesIdx = 0;
            for (int j = 0; j < task.A.ColCount; j++)
            {
                int Num_Row = 0;
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
                        Num_Row = i;
                        isBasis = true;
                    }
                }
                //checked row. Now, if it's basis, we go to this block.
                for (int i = 0; i < task.A.ColCount; i++)
                {
                    if (basis.VectorIndexes.Contains(i) && task.A[Num_Row][i] == task.A[Num_Row][j] && MTaskBasisFinder.Proverka(basis.VectorIndexes))
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
            if(indexesIdx!=basis.VectorIndexes.Length)
                throw new Exception("Basis wasn't found correctly");
            return basis;
        }
    }
}