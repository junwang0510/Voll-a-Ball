using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;


public class MeshGraph
{
    public readonly int Size;
    public readonly int NGon;
    public readonly Vector3[] Vertices;

    private readonly Dictionary<Pair<int>, Pair<int>> _cPairToVPair;
    private readonly HashSet<int>[] _paths;
    private readonly HashSet<int>[] _walls;
    
    public MeshGraph(GeneralMesh mesh)
    {
        if (mesh.primitives.Length % mesh.nGon != 0)
            throw new ArgumentException("Primitives not divisible by nGon.");

        Size = mesh.primitives.Length / mesh.nGon;
        NGon = mesh.nGon;
        Vertices = mesh.vertices;

        var vPairToCells = new Dictionary<Pair<int>, List<int>>();
        // For each cell,
        for (var i = 0; i < Size; i++)
        {
            // For each vPair (u, v) of the cell,
            for (var j = 0; j < NGon; j++)
            {
                var u = i * NGon + j;
                var v = j == NGon - 1 ? 0 : u + 1;
                // Map vPair to cell.
                var vPair = new Pair<int>(u, v);
                if (!vPairToCells.ContainsKey(vPair))
                    vPairToCells[vPair] = new List<int>();
                vPairToCells[vPair].Add(i);
            }
        }

        _cPairToVPair = new Dictionary<Pair<int>, Pair<int>>();
        // For each (vPair, cells that take vPair as their side),
        foreach (var item in vPairToCells)
        {
            // If given vPair is shared by less than 2 cells, NOT an edge.
            if (item.Value.Count != 2)
                continue;
            // Edge detected; cPair = (cell0, cell1).
            var cPair = new Pair<int>(item.Value[0], item.Value[1]);
            _cPairToVPair[cPair] = item.Key;
        }
        
        _paths = new HashSet<int>[Size];
        _walls = new HashSet<int>[Size];
        for (var i = 0; i < Size; i++)
        {
            _paths[i] = new HashSet<int>();
            _walls[i] = new HashSet<int>();
        }
        
        foreach (var cPair in _cPairToVPair.Keys)
        {
            // Each cPair forms a wall.
            var first = cPair.First; var second = cPair.Second;
            _walls[first].Add(second);
            _walls[second].Add(first);
        }
    }
    
    
    /// <summary>
    ///     Removes a wall and adds a path between the two given cells.
    /// </summary>
    /// <param name="cell1">Cell id 1</param>
    /// <param name="cell2">Cell id 2</param>
    public void RemoveWall(int cell1, int cell2)
    {
        _walls[cell1].Remove(cell2);
        _walls[cell2].Remove(cell1);
        _paths[cell1].Add(cell2);
        _paths[cell2].Add(cell1);
    }
    
    
    /// <summary>
    /// Fetches the neighboring paths of a given cell.
    /// </summary>
    /// <param name="cell">Cell id.</param>
    /// <returns>Set of cell ids that are paths away from the cell.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public HashSet<int> GetNeighboringPaths(int cell)
    {
        if (cell < 0 || cell >= Size)
            throw new ArgumentOutOfRangeException();
        return new HashSet<int>(_paths[cell]);  // Creates a copy.
    }

    
    
    /// <summary>
    /// Fetches the neighboring walls of a given cell.
    /// </summary>
    /// <param name="cell">Cell id.</param>
    /// <returns>Set of cell ids that are walls neighboring the cell.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public HashSet<int> GetNeighboringWalls(int cell)
    {
        if (cell < 0 || cell >= Size)
            throw new ArgumentOutOfRangeException();
        return new HashSet<int>(_walls[cell]);  // Creates a copy.
    }
    
    
    /// <summary>
    /// Converts walls to vertex pairs.
    /// </summary>
    /// <returns>Pairs (2-tuple) of 3D vertices where walls need to be formed.</returns>
    public Tuple<Vector3, Vector3>[] WallsToVertexPairs()
    {
        // Set of vertex pairs where a wall needs to be made.
        var vPairs = new HashSet<Pair<int>>();
        for (int ci1 = 0; ci1 < Size; ci1++)
        {
            foreach (var ci2 in _walls[ci1])
                vPairs.Add(_cPairToVPair[new Pair<int>(ci1, ci2)]);
        }
        
        // Array of 2-tuples of Vertex3 where a wall needs to be made.
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


/// <summary>
/// Unordered pair of two items.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Pair<T> : IEquatable<Pair<T>>
{
    public T First { get; }
    public T Second { get; }

    public Pair(T first, T second)
    {
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

        // Combine the hash codes in a way that is order-independent
        return hashFirst ^ hashSecond;
    }
}
