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

    /// <summary>Gets the nodes that no edge ends at (in-degree 0), in slot order.</summary>
    /// <returns>A snapshot: later changes to the graph do not affect it.</returns>
    IReadOnlyCollection<TNode> GetSources();

    /// <summary>Gets the nodes that no edge starts at (out-degree 0), in slot order.</summary>
    /// <returns>A snapshot: later changes to the graph do not affect it.</returns>
    IReadOnlyCollection<TNode> GetSinks();

    /// <summary>Determines whether a path of one or more edges leads from <paramref name="source"/> to <paramref name="target"/>.</summary>
    /// <param name="source">The node the path starts at.</param>
    /// <param name="target">The node the path ends at.</param>
    /// <returns>
    /// <see langword="true"/> if such a path exists; otherwise <see langword="false"/>, including when either node is
    /// missing. A node has a path to itself only through a cycle.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="target"/> is <see langword="null"/>.</exception>
    bool HasPath(TNode source, TNode target);
}
