using System;
using System.Collections.Generic;

public class MazeGenerator
{
    private static readonly Random Rand = new Random();
    private readonly Dictionary<int, List<int>> _graph;
    private readonly int _size;
    private Dictionary<int, List<int>> _maze;

    
    public MazeGenerator(Dictionary<int, List<int>> graph, int size)
    {
        _graph = graph;
        _size = size;
    }

    
    private void Reset()
    {
        _maze = new Dictionary<int, List<int>>();
    }

    
    public Dictionary<int, List<int>> Generate(int initialCell)
    {
        if (initialCell >= _size)
            throw new ArgumentException();

        Reset();

        var stack = new Stack<int>();
        var visited = new HashSet<int>();

        visited.Add(initialCell);
        stack.Push(initialCell);

        while (stack.Count > 0)
        {
            var cell = stack.Pop();
            var neighbors = GetUnvisitedNeighbors(cell, visited);
            if (neighbors.Count > 0)
            {
                stack.Push(cell);
                var chosenCell = neighbors[Rand.Next(neighbors.Count)];
                AddPath(cell, chosenCell);
                visited.Add(chosenCell);
                stack.Push(chosenCell);
            }
        }

        return _maze;
    }


    private List<int> GetUnvisitedNeighbors(int cell, HashSet<int> visited)
    {
        if (cell >= _size) throw new ArgumentException();

        var neighbors = new List<int>();
        if (!_graph.ContainsKey(cell)) return neighbors;
        foreach (var neighbor in _graph[cell])
        {
            if (!visited.Contains(neighbor))
                neighbors.Add(neighbor);
        }

        return neighbors;
    }


    private void AddPath(int start, int end)
    {
        if (!_maze.ContainsKey(start))
            _maze[start] = new List<int>();
        if (!_maze.ContainsKey(end))
            _maze[end] = new List<int>();

        _maze[start].Add(end);
        _maze[start].Add(end);
    }


    public List<int> Solve(int start, int end)
    {
        return Solve(_maze, start, end);
    }


    static List<int> Solve(Dictionary<int, List<int>> maze, int start, int end)
    {
        var queue = new Queue<int>();
        var visited = new HashSet<int>();
        var parentMap = new Dictionary<int, int>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current == end)
                return ReconstructPath(parentMap, start, end);

            foreach (var neighbor in maze[current])
            {
                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    parentMap[neighbor] = current;
                }
            }
        }

        return null; // No path found
    }


    static List<int> ReconstructPath(Dictionary<int, int> parentMap, int start, int end)
    {
        var path = new List<int>();
        var current = end;
        while (!current.Equals(start))
        {
            path.Add(current);
            current = parentMap[current];
        }

        path.Add(start);
        path.Reverse();
        return path;
    }


    public Dictionary<int, List<int>> GetMaze() { return _maze; }

    public int GetSize() { return _size; }

    public Dictionary<int, List<int>> GetGraph() { return _graph; }
}
