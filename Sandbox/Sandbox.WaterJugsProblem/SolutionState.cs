namespace Sandbox.WaterJugsProblem
{
    /// <summary>
    /// The SolutionState struct is a simple structure representing the state of the problem at a given point.
    /// </summary>
    public struct SolutionState
    {
        private int action;
        private int volumeA;
        private int volumeB;

        public int Action
        {
            get { return action; }
            set { action = value; }
        }

        public int VolumeA
        {
            get { return volumeA; }
            set { volumeA = value; }
        }

        public int VolumeB
        {
            get { return volumeB; }
            set { volumeB = value; }
        }
    }
}
