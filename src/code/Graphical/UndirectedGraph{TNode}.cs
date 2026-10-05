using System;
using System.Collections.Generic;
using System.Linq;

namespace Graphical;

/// <summary>A graph whose edges have no direction.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
public sealed class UndirectedGraph<TNode> : Graph<TNode>
    where TNode : notnull
{
    /// <summary>Creates an empty graph that compares nodes with <see cref="EqualityComparer{T}.Default"/>.</summary>
    public UndirectedGraph()
        : this(0, null)
    {
    }

    /// <summary>Creates an empty graph with room for <paramref name="nodeCapacity"/> nodes before it grows.</summary>
    /// <param name="nodeCapacity">The number of nodes the graph can hold before it grows.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nodeCapacity"/> is negative.</exception>
    public UndirectedGraph(int nodeCapacity)
        : this(nodeCapacity, null)
    {
    }

    /// <summary>Creates an empty graph that compares nodes with <paramref name="comparer"/>.</summary>
    /// <param name="comparer">The node comparer, or <see langword="null"/> for <see cref="EqualityComparer{T}.Default"/>.</param>
    public UndirectedGraph(IEqualityComparer<TNode>? comparer)
        : this(0, comparer)
    {
    }

    /// <summary>Creates an empty graph with the given initial capacity and node comparer.</summary>
    /// <param name="nodeCapacity">The number of nodes the graph can hold before it grows.</param>
    /// <param name="comparer">The node comparer, or <see langword="null"/> for <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nodeCapacity"/> is negative.</exception>
    public UndirectedGraph(int nodeCapacity, IEqualityComparer<TNode>? comparer)
        : base(nodeCapacity, comparer)
    {
    }

    private protected override IEnumerable<Edge<TNode>> EnumerateEdgesCore()
    {
        return Enumerable.Empty<Edge<TNode>>();
    }
}
