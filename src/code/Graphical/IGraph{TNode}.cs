using System;
using System.Collections.Generic;

namespace Graphical;

/// <summary>A graph whose nodes and edges can be changed.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
/// <remarks>
/// Graphs are simple: there is at most one edge from a given source to a given target. Implementations may reject
/// edges that would break their own invariants (a <see cref="DirectedAcyclicGraph{TNode}"/> rejects cycles) by
/// throwing an <see cref="InvalidOperationException"/>-derived exception such as <see cref="GraphCycleException"/>;
/// a rejected change leaves the graph exactly as it was.
/// </remarks>
public interface IGraph<TNode> : IReadOnlyGraph<TNode>
    where TNode : notnull
{
    /// <summary>Adds <paramref name="node"/> to the graph.</summary>
    /// <param name="node">The node to add.</param>
    /// <returns><see langword="true"/> if the node was added; <see langword="false"/> if it was already present.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Amortized O(1); growing per-node storage is O(V), or O(V²/64) for a
    /// <see cref="ReachabilityDirectedAcyclicGraph{TNode}"/>.
    /// </remarks>
    bool AddNode(TNode node);

    /// <summary>Adds each node in <paramref name="nodes"/> that is not already present.</summary>
    /// <param name="nodes">The nodes to add; duplicates are ignored.</param>
    /// <returns>The number of nodes actually added.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="nodes"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="nodes"/> contains <see langword="null"/>; nothing is added.</exception>
    /// <remarks>O(n) amortized for n nodes. The whole collection is checked before anything is added.</remarks>
    int AddNodes(IEnumerable<TNode> nodes);

    /// <summary>Removes <paramref name="node"/> and every edge that touches it.</summary>
    /// <param name="node">The node to remove.</param>
    /// <returns><see langword="true"/> if the node was removed; <see langword="false"/> if it was not in the graph.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// O(degree); for a <see cref="ReachabilityDirectedAcyclicGraph{TNode}"/>, O(V + E) plus recomputing the closure
    /// of the node's ancestors and descendants, O((V + E) · V/64) in the worst case.
    /// </remarks>
    bool RemoveNode(TNode node);

    /// <summary>Adds an edge from <paramref name="source"/> to <paramref name="target"/>, adding either node if it is missing.</summary>
    /// <param name="source">The node the edge starts at.</param>
    /// <param name="target">The node the edge ends at.</param>
    /// <returns><see langword="true"/> if the edge was added; <see langword="false"/> if it already existed.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="target"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// The implementation rejects the edge because it would break the graph's invariants; for example a
    /// <see cref="DirectedAcyclicGraph{TNode}"/> throws <see cref="GraphCycleException"/>. The graph is unchanged,
    /// and missing endpoints are not added.
    /// </exception>
    /// <remarks>
    /// Amortized O(1) for undirected and directed graphs; O(V + E) for a <see cref="DirectedAcyclicGraph{TNode}"/>
    /// (cycle check); O(V²/64) in the worst case for a <see cref="ReachabilityDirectedAcyclicGraph{TNode}"/>
    /// (closure update).
    /// </remarks>
    bool AddEdge(TNode source, TNode target);

    /// <summary>Adds each edge in <paramref name="edges"/> that is not already present, adding missing endpoints.</summary>
    /// <param name="edges">The edges to add; duplicates are ignored.</param>
    /// <returns>The number of edges actually added.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="edges"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">An edge has a <see langword="null"/> endpoint, such as <c>default(Edge&lt;TNode&gt;)</c>; nothing is added.</exception>
    /// <exception cref="InvalidOperationException">
    /// The implementation rejects the batch because it would break the graph's invariants; for example a
    /// <see cref="DirectedAcyclicGraph{TNode}"/> throws <see cref="GraphCycleException"/> if the batch would create a
    /// cycle, even one formed only by edges within the batch. The graph is unchanged.
    /// </exception>
    /// <remarks>
    /// O(k) amortized for k edges in undirected and directed graphs; O(V + E + k) for a
    /// <see cref="DirectedAcyclicGraph{TNode}"/>, plus the closure update for a
    /// <see cref="ReachabilityDirectedAcyclicGraph{TNode}"/>. The whole batch is checked before anything is added.
    /// </remarks>
    int AddEdges(IEnumerable<Edge<TNode>> edges);

    /// <summary>Removes the edge from <paramref name="source"/> to <paramref name="target"/>. Both nodes stay in the graph.</summary>
    /// <param name="source">The node the edge starts at.</param>
    /// <param name="target">The node the edge ends at.</param>
    /// <returns>
    /// <see langword="true"/> if the edge was removed; <see langword="false"/> if it did not exist, including when
    /// either node is missing.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="target"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// O(1); for a <see cref="ReachabilityDirectedAcyclicGraph{TNode}"/>, O((V + E) · V/64) in the worst case
    /// (closure recomputation).
    /// </remarks>
    bool RemoveEdge(TNode source, TNode target);

    /// <summary>Removes every node and edge.</summary>
    /// <remarks>O(V); O(V²/64) for a <see cref="ReachabilityDirectedAcyclicGraph{TNode}"/>. Capacity is kept.</remarks>
    void Clear();
}
