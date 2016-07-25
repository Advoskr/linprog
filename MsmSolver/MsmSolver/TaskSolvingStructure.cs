using System.Xml;

namespace MsmSolver
{
    //TODO Need better name
    public class TaskSolvingData
    {
        public Basis Basis { get; set; }
        //public Vector Deltas { get; set; }
        public Vector Lambda { get; set; }
        public Vector X0 { get; set; }
        
    }
}