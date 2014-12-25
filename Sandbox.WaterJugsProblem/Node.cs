namespace Sandbox.WaterJugsProblem
{
    /// <summary>
    /// The Node class creates our representation of a node in our search tree.  Each instantiation has problem
    /// state (using two jug objects), an action, depth in the tree, and a pointer to its parent node.
    /// </summary>
    public class Node
    {
        private Jug jugA;
        private Jug jugB;
        private int depth;
        private int action;
        private Node parent;

        public Jug JugA
        {
            get { return jugA; }
            set { jugA = value; }
        }

        public Jug JugB
        {
            get { return jugB; }
            set { jugB = value; }
        }
        
        public int Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        public int Action
        {
            get { return action; }
            set { action = value; }
        }

        public Node Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public Node(int volumeA, int volumeB, int nodeDepth, int nodeAction, Node parentNode)
        {
            jugA = new Jug(volumeA);
            jugB = new Jug(volumeB);
            depth = nodeDepth;
            action = nodeAction;
            parent = parentNode;
        }
    }
}
