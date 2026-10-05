using System.Collections.Generic;

namespace Graphical;

/// <summary>A directed graph of vertices.</summary>
/// <typeparam name="T">The vertex type.</typeparam>
public class DirectedGraph<T>
{
    private readonly Dictionary<T, HashSet<T>> _adjacencyList;

    /// <summary>Creates an empty graph.</summary>
    public DirectedGraph()
    {
        _adjacencyList = new Dictionary<T, HashSet<T>>();
    }

    /// <summary>Adds a vertex.</summary>
    /// <param name="vertex">The vertex to add.</param>
    public void AddVertex(T vertex)
    {
        _adjacencyList.Add(vertex, new HashSet<T>());
    }

    /// <summary>Adds an edge.</summary>
    /// <param name="source">The source vertex.</param>
    /// <param name="destination">The destination vertex.</param>
    public void AddEdge(T source, T destination)
    {
        _adjacencyList[source].Add(destination);
    }

    /// <summary>Gets the vertices.</summary>
    /// <returns>The vertices.</returns>
    public IEnumerable<T> GetVertices()
    {
        return _adjacencyList.Keys;
    }

    /// <summary>Gets the neighbors of a vertex.</summary>
    /// <param name="vertex">The vertex.</param>
    /// <returns>The neighbors.</returns>
    public IEnumerable<T> GetNeighbors(T vertex)
    {
        return _adjacencyList[vertex];
    }
}
