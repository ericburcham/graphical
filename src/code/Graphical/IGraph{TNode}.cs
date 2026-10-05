using System.Collections.Generic;

namespace Graphical;

/// <summary>A graph whose nodes and edges can be changed.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
public interface IGraph<TNode> : IReadOnlyGraph<TNode>
    where TNode : notnull
{
    /// <summary>Adds <paramref name="node"/> to the graph.</summary>
    /// <param name="node">The node to add.</param>
    /// <returns><see langword="true"/> if the node was added; <see langword="false"/> if it was already present.</returns>
    bool AddNode(TNode node);

    /// <summary>Adds each node in <paramref name="nodes"/> that is not already present.</summary>
    /// <param name="nodes">The nodes to add; duplicates are ignored.</param>
    /// <returns>The number of nodes actually added.</returns>
    int AddNodes(IEnumerable<TNode> nodes);

    /// <summary>Removes <paramref name="node"/> and every edge that touches it.</summary>
    /// <param name="node">The node to remove.</param>
    /// <returns><see langword="true"/> if the node was removed; <see langword="false"/> if it was not in the graph.</returns>
    bool RemoveNode(TNode node);

    /// <summary>Adds an edge from <paramref name="source"/> to <paramref name="target"/>, adding either node if it is missing.</summary>
    /// <param name="source">The node the edge starts at.</param>
    /// <param name="target">The node the edge ends at.</param>
    /// <returns><see langword="true"/> if the edge was added; <see langword="false"/> if it already existed.</returns>
    bool AddEdge(TNode source, TNode target);

    /// <summary>Removes the edge from <paramref name="source"/> to <paramref name="target"/>. Both nodes stay in the graph.</summary>
    /// <param name="source">The node the edge starts at.</param>
    /// <param name="target">The node the edge ends at.</param>
    /// <returns><see langword="true"/> if the edge was removed; <see langword="false"/> if it did not exist, including when either node is missing.</returns>
    bool RemoveEdge(TNode source, TNode target);

    /// <summary>Adds each edge in <paramref name="edges"/> that is not already present, adding missing endpoints.</summary>
    /// <param name="edges">The edges to add; duplicates are ignored.</param>
    /// <returns>The number of edges actually added.</returns>
    int AddEdges(IEnumerable<Edge<TNode>> edges);
}
