using System;
using System.Collections.Generic;

namespace Graphical;

/// <summary>A read-only view of a graph whose edges point from a source node to a target node.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
public interface IReadOnlyDirectedGraph<TNode> : IReadOnlyGraph<TNode>
    where TNode : notnull
{
    /// <summary>Gets a live read-only view of the nodes that <paramref name="node"/> has an edge to.</summary>
    /// <param name="node">The node whose successors to get.</param>
    /// <returns>The successors.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException"><paramref name="node"/> is not in the graph.</exception>
    IReadOnlyCollection<TNode> GetSuccessors(TNode node);

    /// <summary>Gets a live read-only view of the nodes that have an edge to <paramref name="node"/>.</summary>
    /// <param name="node">The node whose predecessors to get.</param>
    /// <returns>The predecessors.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException"><paramref name="node"/> is not in the graph.</exception>
    IReadOnlyCollection<TNode> GetPredecessors(TNode node);

    /// <summary>Gets the number of edges that end at <paramref name="node"/>.</summary>
    /// <param name="node">The node whose in-degree to get.</param>
    /// <returns>The in-degree.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException"><paramref name="node"/> is not in the graph.</exception>
    int GetInDegree(TNode node);

    /// <summary>Gets the number of edges that start at <paramref name="node"/>.</summary>
    /// <param name="node">The node whose out-degree to get.</param>
    /// <returns>The out-degree.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException"><paramref name="node"/> is not in the graph.</exception>
    int GetOutDegree(TNode node);
}
