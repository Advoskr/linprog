using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MsmSolver
{
    public class Task
    {
        public Matrix A { get; set; }
        public Vector C { get; set; }
        public Vector A0 { get; set; }

        public Signs[] Signs { get; set; }

        public Direction Direction { get; set; }

        public Task()
        {

        }

        public Task(Vector c, Matrix a, Vector a0, Signs[] s, Direction dir)
        {
            this.C = c;
            this.A = a;
            this.A0 = a0;
            this.Signs = s;
            this.Direction = dir;
        }
    }
}
