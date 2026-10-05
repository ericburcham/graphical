using System;
using System.Collections.Generic;

namespace Graphical;

/// <summary>A read-only view of a graph: its nodes, its edges and how they connect.</summary>
/// <typeparam name="TNode">
/// The node type. Nodes are compared with <see cref="Comparer"/>; a node's equality and hash code must not change
/// while it is in a graph.
/// </typeparam>
/// <remarks>
/// Time complexities use V for the number of nodes and E for the number of edges. Members that return views
/// (<see cref="Nodes"/>, <see cref="Edges"/>, <see cref="GetNeighbors"/>) return live views: they reflect later
/// changes, and an enumeration in progress throws <see cref="InvalidOperationException"/> if the graph changes.
/// </remarks>
public interface IReadOnlyGraph<TNode>
    where TNode : notnull
{
    /// <summary>Gets a value indicating whether the graph's edges have a direction.</summary>
    /// <remarks>O(1).</remarks>
    bool IsDirected { get; }

    /// <summary>Gets the comparer used to decide whether two nodes are the same node.</summary>
    /// <remarks>
    /// O(1). <see cref="Edge{TNode}"/> equality does not use this comparer; it always uses
    /// <see cref="EqualityComparer{T}.Default"/>.
    /// </remarks>
    IEqualityComparer<TNode> Comparer { get; }

    /// <summary>Gets the number of nodes in the graph.</summary>
    /// <remarks>O(1).</remarks>
    int NodeCount { get; }

    /// <summary>Gets the number of edges in the graph. An undirected edge, including a self-loop, counts once.</summary>
    /// <remarks>O(1).</remarks>
    int EdgeCount { get; }

    /// <summary>Gets a live read-only view of the nodes in the graph, in slot order.</summary>
    /// <remarks>
    /// Getting the view and its count is O(1); enumerating it is O(V). Slot order is insertion order until a node is
    /// removed; a node added later reuses the most recently freed slot.
    /// </remarks>
    IReadOnlyCollection<TNode> Nodes { get; }

    /// <summary>Gets a live read-only view of the edges in the graph.</summary>
    /// <remarks>
    /// Getting the view and its count is O(1); enumerating it is O(V + E). Each edge appears once; in an undirected
    /// graph its <see cref="Edge{TNode}.Source"/> is the endpoint that was added to the graph first.
    /// </remarks>
    IReadOnlyCollection<Edge<TNode>> Edges { get; }

    /// <summary>Determines whether the graph contains <paramref name="node"/>.</summary>
    /// <param name="node">The node to look for.</param>
    /// <returns><see langword="true"/> if the graph contains the node; otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
    /// <remarks>O(1).</remarks>
    bool ContainsNode(TNode node);

    /// <summary>Determines whether the graph contains an edge from <paramref name="source"/> to <paramref name="target"/>.</summary>
    /// <param name="source">The node the edge starts at.</param>
    /// <param name="target">The node the edge ends at.</param>
    /// <returns>
    /// <see langword="true"/> if the edge exists (in either direction for an undirected graph); otherwise
    /// <see langword="false"/>, including when either node is missing.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="target"/> is <see langword="null"/>.</exception>
    /// <remarks>O(1).</remarks>
    bool ContainsEdge(TNode source, TNode target);

    /// <summary>Gets a live read-only view of the nodes that share an edge with <paramref name="node"/>, each once.</summary>
    /// <param name="node">The node whose neighbors to get.</param>
    /// <returns>
    /// The neighbors; for a directed graph, successors and predecessors together. A node with a self-loop is its own
    /// neighbor. If the node is later removed, the view is empty.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException"><paramref name="node"/> is not in the graph.</exception>
    /// <remarks>
    /// O(1) to get the view. Enumerating it is O(degree). Its count is O(1) for an undirected graph and O(in-degree)
    /// for a directed one.
    /// </remarks>
    IReadOnlyCollection<TNode> GetNeighbors(TNode node);

    /// <summary>Gets the number of edge endpoints at <paramref name="node"/>.</summary>
    /// <param name="node">The node whose degree to get.</param>
    /// <returns>The degree: a self-loop counts twice, and for a directed graph this is in-degree plus out-degree.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException"><paramref name="node"/> is not in the graph.</exception>
    /// <remarks>O(1).</remarks>
    int GetDegree(TNode node);
}
