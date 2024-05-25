using System;
using System.Collections.Generic;
using UnityEngine;

// Class representing a graph structure for a mesh
public class MeshGraph
{
    public readonly int Size; // Number of cells in the mesh
    public readonly int NGon; // Number of vertices per cell
    public readonly Vector3[] Vertices; // Array of vertices
    private readonly Dictionary<Pair<int>, Pair<int>> _cPairToVPair; // Mapping from cell pairs to vertex pairs
    private readonly HashSet<int>[] _paths; // Paths between cells
    private readonly HashSet<int>[] _walls; // Walls between cells

    public MeshGraph(GeneralMesh mesh)
    {
        if (mesh.primitives.Length % mesh.nGon != 0)
            throw new ArgumentException("Primitives not divisible by nGon.");

        // Initialize properties
        Size = mesh.primitives.Length / mesh.nGon;
        NGon = mesh.nGon;
        Vertices = mesh.vertices;

        // Initialize a dictionary to store vertex pairs and their corresponding cells
        var vPairToCells = new Dictionary<Pair<int>, HashSet<int>>();
        for (var i = 0; i < Size; i++)
        {
            for (var j = 0; j < NGon; j++)
            {
                var u = mesh.primitives[i * NGon + j];
                var v = mesh.primitives[i * NGon + ((j + 1) % NGon)];
                var vPair = new Pair<int>(u, v);
                if (!vPairToCells.ContainsKey(vPair))
                    vPairToCells[vPair] = new HashSet<int>();
                vPairToCells[vPair].Add(i);
            }
        }

        // BEGIN: DEBUG (Works as expected)
        // Debug.Log("vPairToCells");
        // foreach (var (vPair, Cells) in vPairToCells)
        // {
        //     Debug.Log($"vPair: ({vPair.First}, {vPair.Second})");
        //     var msg = "Cells: {";
        //     foreach (var cell in Cells) msg += cell + " ";
        //     Debug.Log(msg + "}");
        // }
        // END: DEBUG

        // Map cell pairs to vertex pairs
        _cPairToVPair = new Dictionary<Pair<int>, Pair<int>>();
        foreach (var (vPair, cellSet) in vPairToCells)
        {
            // If not an edge, continue.
            if (cellSet.Count != 2)
                continue;

            var cellList = new List<int>(cellSet);
            var cPair = new Pair<int>(cellList[0], cellList[1]);
            _cPairToVPair[cPair] = vPair;
        }

        // Initialize paths and walls
        _paths = new HashSet<int>[Size];
        _walls = new HashSet<int>[Size];
        for (var i = 0; i < Size; i++)
        {
            _paths[i] = new HashSet<int>();
            _walls[i] = new HashSet<int>();
        }

        // Set up walls based on cell pairs
        foreach (var cPair in _cPairToVPair.Keys)
        {
            _walls[cPair.First].Add(cPair.Second);
            _walls[cPair.Second].Add(cPair.First);
        }

        // BEGIN: DEBUG (Works as expected)
        // for (var i = 0; i < Size; i++)
        // {
        //     Debug.Log($"Cell: {i}");
        //     var msg = "Neighbors: { ";
        //     foreach (var wall in _walls[i])
        //         msg += wall + " ";
        //     Debug.Log(msg + "}");
        // }
        // END: DEBUG
    }

    // Replace a wall between two cells with a path
    public void RemoveWall(int cell1, int cell2)
    {
        _walls[cell1].Remove(cell2);
        _walls[cell2].Remove(cell1);
        _paths[cell1].Add(cell2);
        _paths[cell2].Add(cell1);
    }

    // Get neighboring paths of a cell
    public HashSet<int> GetNeighboringPaths(int cell)
    {
        if (cell < 0 || cell >= Size)
            throw new ArgumentOutOfRangeException();
        return new HashSet<int>(_paths[cell]);
    }

    // Get neighboring walls of a cell
    public HashSet<int> GetNeighboringWalls(int cell)
    {
        if (cell < 0 || cell >= Size)
            throw new ArgumentOutOfRangeException();
        return new HashSet<int>(_walls[cell]);
    }

    // Walls -> vertex pairs
    public Tuple<Vector3, Vector3>[] WallsToVertexPairs()
    {
        var vPairs = new HashSet<Pair<int>>();
        for (var ci1 = 0; ci1 < Size; ci1++)
        {
            foreach (var ci2 in _walls[ci1])
                vPairs.Add(_cPairToVPair[new Pair<int>(ci1, ci2)]);
        }

        var res = new Tuple<Vector3, Vector3>[vPairs.Count];
        var i = 0;
        foreach (var pair in vPairs)
        {
            var vi1 = pair.First; var vi2 = pair.Second;
            res[i] = new Tuple<Vector3, Vector3>(Vertices[vi1], Vertices[vi2]);
            i++;
        }
        return res;
    }
}

// Class representing a pair of values
public class Pair<T> : IEquatable<Pair<T>>
{
    public T First { get; }
    public T Second { get; }

    public Pair(T first, T second)
    {
        // Ensure the first value is always less than or equal to the second
        if (Comparer<T>.Default.Compare(first, second) > 0)
        {
            First = second;
            Second = first;
        }
        else
        {
            First = first;
            Second = second;
        }
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Pair<T>);
    }

    public bool Equals(Pair<T> other)
    {
        if (other == null)
            return false;

        return EqualityComparer<T>.Default.Equals(First, other.First) &&
               EqualityComparer<T>.Default.Equals(Second, other.Second);
    }

    public override int GetHashCode()
    {
        int hashFirst = EqualityComparer<T>.Default.GetHashCode(First);
        int hashSecond = EqualityComparer<T>.Default.GetHashCode(Second);

        return hashFirst ^ hashSecond;
    }
}
