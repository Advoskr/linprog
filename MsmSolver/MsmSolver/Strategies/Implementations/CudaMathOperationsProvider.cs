using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsmSolver.Strategies.Implementations
{
    class CudaMathOperationsProvider : SingleCoreMathOperationsProvider
    {
      /*  public void Math_Mul(Matrix a)
        {
            double[,] vC = new double[a.ColCount, this.RowCount];
            externExtensions.MatMul(vC, a._values, this._values, a.RowCount, this.ColCount, a.ColCount);
            _values = vC;
        }

        public void Math_Mul_Tile(Matrix a)
        {
            double[,] vC = new double[a.ColCount, this.RowCount];
            externExtensions.MatMulFire(vC, a._values, this._values, a.RowCount, this.ColCount, a.ColCount);
            _values = vC;
        }*/
    }
}
