using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Algorithms.Services;
using QuickGraph.Algorithms.Search;
using QuickGraph.Collections;
using QuickGraph.Data;

namespace Sandbox.QuickGraph
{
    /// <summary>
    /// Learning exercises for the QuickGraph library.
    /// </summary>
    public static class QuickGraphExamples
    {
        public static void Run()
        {
            Console.WriteLine("QuickGraph examples");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            // Create a graph
            var g = new AdjacencyGraph<int, TaggedEdge<int, string>>();

            // Add vertices
            int v1 = 1;
            int v2 = 2;
            g.AddVertex(v1);
            g.AddVertex(v2);

            // Add an edge
            var e1 = new TaggedEdge<int,string>(v1, v2, "hello");
            g.AddEdge(e1);

            // Add edge and vertices at the same time
            var e2 = new TaggedEdge<int, string>(3, 4, "hello");
            g.AddVerticesAndEdge(e2);

            // Iterate vertices
            Console.WriteLine("Vertices:");
            foreach (var v in g.Vertices)
                Console.WriteLine(v);

            // Iterate edges
            Console.WriteLine("");
            Console.WriteLine("Edges:");
            foreach (var e in g.Edges)
                Console.WriteLine(e);

            // Iterate out edges
            Console.WriteLine("");
            Console.WriteLine("Out Edges:");
            foreach (var v in g.Vertices)
                foreach (var e in g.OutEdges(v))
                    Console.WriteLine(e);

            // Depth-first search
            var dfs = new DepthFirstSearchAlgorithm<int, TaggedEdge<int, string>>(g);
            dfs.Compute();

            Console.WriteLine("");

            Console.WriteLine("Example complete. Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }
    }
}
