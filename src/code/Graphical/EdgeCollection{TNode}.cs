using System.Collections;
using System.Collections.Generic;

namespace Graphical;

/// <summary>A live read-only view of the edges in a graph.</summary>
internal sealed class EdgeCollection<TNode> : IReadOnlyCollection<Edge<TNode>>
    where TNode : notnull
{
    private readonly Graph<TNode> _graph;

    public EdgeCollection(Graph<TNode> graph)
    {
        _graph = graph;
    }

    public int Count => _graph.EdgeCount;

    public IEnumerator<Edge<TNode>> GetEnumerator()
    {
        var version = _graph.Version;
        foreach (var edge in _graph.EnumerateEdges())
        {
            ThrowHelper.ThrowIfVersionChanged(version, _graph.Version);
            yield return edge;
        }

        ThrowHelper.ThrowIfVersionChanged(version, _graph.Version);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
