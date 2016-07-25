namespace MsmSolver
{
    public class Answer
    {
        public Basis Basis { get; set; }

        public double Z { get; set; }

        public int StepCount { get; set; }

        public string SolvingMethod { get; set; }

        public Vector Solution { get; set; }
        
    }
}