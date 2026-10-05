using System;
using System.Collections.Generic;

namespace Graphical;

/// <summary>A read-only view of a directed graph that has no cycles.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
public interface IReadOnlyDirectedAcyclicGraph<TNode> : IReadOnlyDirectedGraph<TNode>
    where TNode : notnull
{
    /// <summary>Gets every node ordered so that each edge's source comes before its target.</summary>
    /// <returns>
    /// A read-only snapshot. Ties are broken by insertion order (earlier-added nodes first), so identical sequences of
    /// operations always produce identical orders. An unchanged graph may return the same cached instance.
    /// </returns>
    /// <remarks>O(V log V + E) after a change (Kahn's algorithm with a min-heap on insertion order); O(1) while the graph is unchanged.</remarks>
    IReadOnlyList<TNode> GetTopologicalOrder();

    /// <summary>Gets every node that has a path to <paramref name="node"/>, not including the node itself.</summary>
    /// <param name="node">The node whose ancestors to get.</param>
    /// <returns>A snapshot of the ancestors, in slot order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException"><paramref name="node"/> is not in the graph.</exception>
    /// <remarks>O(V + E) (an iterative breadth-first search) for <see cref="DirectedAcyclicGraph{TNode}"/>; O(V/64 + result) for <see cref="ReachabilityDirectedAcyclicGraph{TNode}"/>.</remarks>
    IReadOnlyCollection<TNode> GetAncestors(TNode node);

    /// <summary>Gets every node that <paramref name="node"/> has a path to, not including the node itself.</summary>
    /// <param name="node">The node whose descendants to get.</param>
    /// <returns>A snapshot of the descendants, in slot order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException"><paramref name="node"/> is not in the graph.</exception>
    /// <remarks>O(V + E) (an iterative breadth-first search) for <see cref="DirectedAcyclicGraph{TNode}"/>; O(V/64 + result) for <see cref="ReachabilityDirectedAcyclicGraph{TNode}"/>.</remarks>
    IReadOnlyCollection<TNode> GetDescendants(TNode node);

    /// <summary>Determines whether adding an edge from <paramref name="source"/> to <paramref name="target"/> would create a cycle.</summary>
    /// <param name="source">The node the edge would start at.</param>
    /// <param name="target">The node the edge would end at.</param>
    /// <returns>
    /// <see langword="true"/> if the nodes are the same (a self-loop), even when missing, or if
    /// <paramref name="target"/> already has a path to <paramref name="source"/>; otherwise <see langword="false"/>,
    /// including when either node is missing.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="target"/> is <see langword="null"/>.</exception>
    /// <remarks>O(V + E) for <see cref="DirectedAcyclicGraph{TNode}"/>; O(1) for <see cref="ReachabilityDirectedAcyclicGraph{TNode}"/>.</remarks>
    bool WouldCreateCycle(TNode source, TNode target);
}
