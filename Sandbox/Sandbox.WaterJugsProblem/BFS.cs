using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandbox.WaterJugsProblem
{
    /// <summary>
    /// The BFS class contains the logic for a breadth-first search on our jug problem.
    /// </summary>
    public class BFS
    {
        /// <summary>
        /// This method takes the user's input from the main body and either returns a stack of sequential states
        /// that represent a solution, or returns an empty stack that shows no solution was found.
        /// </summary>
        public static Stack<SolutionState> FindBFSJugSolution(int volumeA, int volumeB, int solutionVolume)
        {
            //Create our data structures and queue up the root node
            Stack<SolutionState> solutionPath = new Stack<SolutionState>();
            Queue<Node> expansionQueue = new Queue<Node>();
            Node root = new Node(volumeA, volumeB, 0, 0, null);
            expansionQueue.Enqueue(root);

            //While the queue is not empty, continue taking nodes off the queue and checking for
            //a solution state.  If no solution is found, expand the current node.
            while (expansionQueue.Count > 0)
            {
                Node current = expansionQueue.Dequeue();

                //Check to see if the current node is a goal state.  If so, report back to main program
                //with a stack of states reporesenting the solution path.
                if ((current.JugA.CurrentVolume == solutionVolume) || (current.JugB.CurrentVolume == solutionVolume))
                {
                    while (current.Parent != null)
                    {
                        SolutionState currentState = new SolutionState();
                        currentState.Action = current.Action;
                        currentState.VolumeA = current.JugA.CurrentVolume;
                        currentState.VolumeB = current.JugB.CurrentVolume;
                        solutionPath.Push(currentState);
                        current = current.Parent;
                    }

                    return solutionPath;
                }
                else
                {
                    //Current node was not a goal state so expand it and move to the next node
                    Expand(expansionQueue, current);
                }
            }

            //No solution found, return to main program with empty stack
            return solutionPath;
        }

        /// <summary>
        /// This method takes our existing queue as well as the current node, and expands the current node
        /// while adding the new fringe nodes to the queue.
        /// </summary>
        private static void Expand(Queue<Node> expansionQueue, Node current)
        {
            int volumeA = current.JugA.Capacity;
            int volumeB = current.JugB.Capacity;
            int newDepth = current.Depth + 1;

            //We set a depth limit to prevent the program from running forever if no solution exists
            if (newDepth <= 9)
            {
                //For each of the six possible actions, create a new fringe node with the state of
                //its parent node and then update the state by performing the assigned action.
                Node first = new Node(volumeA, volumeB, newDepth, 1, current);
                first.JugA.CurrentVolume = current.JugA.CurrentVolume;
                first.JugB.CurrentVolume = current.JugB.CurrentVolume;
                first.JugA.Fill();
                expansionQueue.Enqueue(first);

                Node second = new Node(volumeA, volumeB, newDepth, 2, current);
                second.JugA.CurrentVolume = current.JugA.CurrentVolume;
                second.JugB.CurrentVolume = current.JugB.CurrentVolume;
                second.JugB.Fill();
                expansionQueue.Enqueue(second);

                Node third = new Node(volumeA, volumeB, newDepth, 3, current);
                third.JugA.CurrentVolume = current.JugA.CurrentVolume;
                third.JugB.CurrentVolume = current.JugB.CurrentVolume;
                third.JugA.Empty();
                expansionQueue.Enqueue(third);

                Node fourth = new Node(volumeA, volumeB, newDepth, 4, current);
                fourth.JugA.CurrentVolume = current.JugA.CurrentVolume;
                fourth.JugB.CurrentVolume = current.JugB.CurrentVolume;
                fourth.JugB.Empty();
                expansionQueue.Enqueue(fourth);

                Node fifth = new Node(volumeA, volumeB, newDepth, 5, current);
                fifth.JugA.CurrentVolume = current.JugA.CurrentVolume;
                fifth.JugB.CurrentVolume = current.JugB.CurrentVolume;
                fifth.JugB.TransferTo(fifth.JugA);
                expansionQueue.Enqueue(fifth);

                Node sixth = new Node(volumeA, volumeB, newDepth, 6, current);
                sixth.JugA.CurrentVolume = current.JugA.CurrentVolume;
                sixth.JugB.CurrentVolume = current.JugB.CurrentVolume;
                sixth.JugA.TransferTo(sixth.JugB);
                expansionQueue.Enqueue(sixth);
            }
        }
    }
}
