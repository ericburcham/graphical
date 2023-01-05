using System.Collections.Generic;

namespace Graphical;

public class DirectedGraph<T>
{
    private readonly Dictionary<T, HashSet<T>> _adjacencyList;

    public DirectedGraph()
    {
        _adjacencyList = new Dictionary<T, HashSet<T>>();
    }

    public void AddVertex(T vertex)
    {
        _adjacencyList.Add(vertex, new HashSet<T>());
    }

    public void AddEdge(T source, T destination)
    {
        _adjacencyList[source].Add(destination);
    }

    public IEnumerable<T> GetVertices()
    {
        return _adjacencyList.Keys;
    }

    public IEnumerable<T> GetNeighbors(T vertex)
    {
        return _adjacencyList[vertex];
    }
}
