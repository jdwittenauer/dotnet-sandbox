using System;
using System.Collections.Generic;

namespace Sandbox.WaterJugsProblem
{
    /// <summary>
    /// This is the main program body for the water jugs solution. It prompts for input
    /// and reports the results of the search.
    /// </summary>
    public static class Launcher
    {
        public static void Run()
        {
            string input1, input2, input3;
            int a, b, x;

            //Get user input and parse into integers for jug size and solution amount
            Console.WriteLine("Size of jug A:");
            input1 = Console.ReadLine();
            Console.WriteLine("Size of jug B:");
            input2 = Console.ReadLine();
            Console.WriteLine("Desired amount:");
            input3 = Console.ReadLine();

            a = int.Parse(input1);
            b = int.Parse(input2);
            x = int.Parse(input3);

            //Start a timer and run the search
            int startTime = System.Environment.TickCount;
            Stack<SolutionState> solutionPath = BFS.FindBFSJugSolution(a, b, x);
            int endTime = System.Environment.TickCount;
            double executionTime = (double)(endTime - startTime) / 1000.0;

            //If no solution found then report to user, otherwise display results
            Console.WriteLine();
            if (solutionPath.Count == 0)
            {
                Console.WriteLine("No solution found after " + executionTime + " seconds. Maximum search depth is 9.");
            }
            else
            {
                Console.WriteLine("Solution found in " + executionTime + " seconds.");
                Console.WriteLine();
                Console.WriteLine("Solution path:");
                Console.WriteLine();

                while (solutionPath.Count > 0)
                {
                    SolutionState item = solutionPath.Pop();
                    switch (item.Action)
                    {
                        case 1:
                            Console.WriteLine("Fill jug A (A=" + item.VolumeA + ", B=" + item.VolumeB + ")");
                            break;
                        case 2:
                            Console.WriteLine("Fill jug B (A=" + item.VolumeA + ", B=" + item.VolumeB + ")");
                            break;
                        case 3:
                            Console.WriteLine("Empty jug A (A=" + item.VolumeA + ", B=" + item.VolumeB + ")");
                            break;
                        case 4:
                            Console.WriteLine("Empty jug B (A=" + item.VolumeA + ", B=" + item.VolumeB + ")");
                            break;
                        case 5:
                            Console.WriteLine("Transfer from jug B to jug A (A=" + item.VolumeA + ", B=" + item.VolumeB + ")");
                            break;
                        case 6:
                            Console.WriteLine("Transfer from jug A to jug B (A=" + item.VolumeA + ", B=" + item.VolumeB + ")");
                            break;
                        default:
                            Console.WriteLine("Action not recognized");
                            break;
                    }
                }
            }
        }
    }
}
