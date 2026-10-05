namespace Graphical;

/// <summary>Factory methods for <see cref="Edge{TNode}"/> that infer the node type.</summary>
public static class Edge
{
    /// <summary>Creates an edge from <paramref name="source"/> to <paramref name="target"/>.</summary>
    /// <typeparam name="TNode">The node type, inferred from the arguments.</typeparam>
    /// <param name="source">The node the edge starts at.</param>
    /// <param name="target">The node the edge ends at.</param>
    /// <returns>The new edge.</returns>
    /// <remarks>O(1).</remarks>
    public static Edge<TNode> Create<TNode>(TNode source, TNode target)
        where TNode : notnull
    {
        return new Edge<TNode>(source, target);
    }
}
