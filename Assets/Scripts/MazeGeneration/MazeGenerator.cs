using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

/// <summary>
/// Generate a maze on the given mesh using DFS.
/// Input: mesh (class GeneralMesh)
/// Output: pairs (2-tuple) of 3D vertices where walls need to be formed
/// </summary>
public class MazeGenerator
{
    /// <summary>
    ///     Design a maze on the given mesh.
    ///     The only function that the client uses.
    ///     TODO: Allow the client to specify minimum path length constraint.
    /// </summary>
    /// <param name="mesh">Input mesh.</param>
    /// <param name="algorithm">Algorithm to run.</param>
    /// <param name="start">Starting cell index.</param>
    /// <returns>Pairs (2-tuple) of 3D vertices where walls need to be formed.</returns>
    /// <exception cref="NotImplementedException">
    ///     The only supported algorithm(s) are: {"DFS"}.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If <c>start</c> is out of range.
    /// </exception>
    public static Tuple<Vector3, Vector3>[] Generate(
        GeneralMesh mesh,
        string algorithm = "DFS",
        int start = 0)
    {
        var graph = new MeshGraph(mesh);

        if (start < 0 || start >= graph.Size)
            throw new ArgumentOutOfRangeException();

        // In-place maze generation via wall removal
        if (algorithm == "DFS")
            GenerateDfs(graph, start);
        else
            throw new NotImplementedException();

        return graph.WallsToVertexPairs();
    }


    /// <summary>
    ///     Generates a maze on <c>graph</c> by removing the walls in-place
    ///     using a DFS RECURSIVE BACKTRACKING algorithm.
    /// </summary>
    /// <param name="graph">Input MeshGraph.</param>
    /// <param name="start">Starting cell. Assumed to be in-range.</param>
    private static void GenerateDfs(MeshGraph graph, int start)
    {
        HashSet<int> GetUnvisitedNeighbors(int cell, HashSet<int> visited)
        {
            var neighbors = graph.GetNeighboringWalls(cell);
            neighbors.ExceptWith(visited);
            return neighbors;
        }

        var rng = new Random();
        var visited = new HashSet<int>();
        var stack = new Stack<int>();

        visited.Add(start);
        stack.Push(start);

        while (stack.Count > 0)
        {
            var cell = stack.Pop();
            var neighbors = GetUnvisitedNeighbors(cell, visited).ToArray();
            if (neighbors.Length == 0) continue;    // No neighbors, then skip.
            stack.Push(cell);
            var chosenCell = neighbors[rng.Next(neighbors.Length)];
            graph.RemoveWall(cell, chosenCell);
            visited.Add(chosenCell);
            stack.Push(chosenCell);
        }
    }
}
