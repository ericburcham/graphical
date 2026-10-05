using System.Collections.Generic;
using System.Linq;

namespace Graphical;

/// <summary>A graph whose edges have no direction.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
public sealed class UndirectedGraph<TNode> : Graph<TNode>
    where TNode : notnull
{
    /// <summary>Creates an empty graph that compares nodes with <paramref name="comparer"/>.</summary>
    /// <param name="comparer">The node comparer, or <see langword="null"/> for <see cref="EqualityComparer{T}.Default"/>.</param>
    public UndirectedGraph(IEqualityComparer<TNode>? comparer)
        : base(comparer)
    {
    }

    private protected override IEnumerable<Edge<TNode>> EnumerateEdgesCore()
    {
        return Enumerable.Empty<Edge<TNode>>();
    }
}
