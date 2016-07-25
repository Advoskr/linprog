namespace MsmSolver
{
    public class Basis
    {
        public Matrix Values { get; set; }
        
        /// <summary>
        /// Indexes of basis variables
        /// </summary>
        public int[] VectorIndexes { get; set; } 
    }
}