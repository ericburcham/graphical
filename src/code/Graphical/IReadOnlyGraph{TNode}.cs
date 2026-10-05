using System;
using System.Collections.Generic;

namespace Graphical;

/// <summary>A read-only view of a graph: its nodes, its edges and how they connect.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
public interface IReadOnlyGraph<TNode>
    where TNode : notnull
{
    /// <summary>Gets a value indicating whether the graph's edges have a direction.</summary>
    bool IsDirected { get; }

    /// <summary>Gets the comparer used to decide whether two nodes are the same node.</summary>
    IEqualityComparer<TNode> Comparer { get; }

    /// <summary>Gets the number of nodes in the graph.</summary>
    int NodeCount { get; }

    /// <summary>Gets the number of edges in the graph.</summary>
    int EdgeCount { get; }

    /// <summary>Gets a live read-only view of the nodes in the graph.</summary>
    IReadOnlyCollection<TNode> Nodes { get; }

    /// <summary>Gets a live read-only view of the edges in the graph.</summary>
    IReadOnlyCollection<Edge<TNode>> Edges { get; }

    /// <summary>Determines whether the graph contains <paramref name="node"/>.</summary>
    /// <param name="node">The node to look for.</param>
    /// <returns><see langword="true"/> if the graph contains the node; otherwise <see langword="false"/>.</returns>
    bool ContainsNode(TNode node);

    /// <summary>Determines whether the graph contains an edge from <paramref name="source"/> to <paramref name="target"/>.</summary>
    /// <param name="source">The node the edge starts at.</param>
    /// <param name="target">The node the edge ends at.</param>
    /// <returns><see langword="true"/> if the edge exists; otherwise <see langword="false"/>, including when either node is missing.</returns>
    bool ContainsEdge(TNode source, TNode target);

    /// <summary>Gets a live read-only view of the nodes that share an edge with <paramref name="node"/>, each once.</summary>
    /// <param name="node">The node whose neighbors to get.</param>
    /// <returns>The neighbors; for a directed graph, successors and predecessors together.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException"><paramref name="node"/> is not in the graph.</exception>
    IReadOnlyCollection<TNode> GetNeighbors(TNode node);

    /// <summary>Gets the number of edge endpoints at <paramref name="node"/>.</summary>
    /// <param name="node">The node whose degree to get.</param>
    /// <returns>The degree; a self-loop counts twice, and for a directed graph this is in-degree plus out-degree.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException"><paramref name="node"/> is not in the graph.</exception>
    int GetDegree(TNode node);
}
