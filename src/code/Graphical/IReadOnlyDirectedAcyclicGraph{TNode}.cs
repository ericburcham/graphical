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
    IReadOnlyList<TNode> GetTopologicalOrder();

    /// <summary>Determines whether adding an edge from <paramref name="source"/> to <paramref name="target"/> would create a cycle.</summary>
    /// <param name="source">The node the edge would start at.</param>
    /// <param name="target">The node the edge would end at.</param>
    /// <returns>
    /// <see langword="true"/> if the nodes are the same (a self-loop), even when missing, or if
    /// <paramref name="target"/> already has a path to <paramref name="source"/>; otherwise <see langword="false"/>,
    /// including when either node is missing.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="target"/> is <see langword="null"/>.</exception>
    bool WouldCreateCycle(TNode source, TNode target);
}
