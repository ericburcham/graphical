namespace Graphical;

/// <summary>A directed pair of nodes: an edge from <see cref="Source"/> to <see cref="Target"/>.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
public readonly struct Edge<TNode>
    where TNode : notnull
{
    /// <summary>Creates an edge from <paramref name="source"/> to <paramref name="target"/>.</summary>
    /// <param name="source">The node the edge starts at.</param>
    /// <param name="target">The node the edge ends at.</param>
    public Edge(TNode source, TNode target)
    {
        Source = source;
        Target = target;
    }

    /// <summary>Gets the node the edge starts at.</summary>
    public TNode Source { get; }

    /// <summary>Gets the node the edge ends at.</summary>
    public TNode Target { get; }
}
