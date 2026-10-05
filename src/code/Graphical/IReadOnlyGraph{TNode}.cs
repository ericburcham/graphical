using System.Collections.Generic;

namespace Graphical;

/// <summary>A read-only view of a graph: its nodes, its edges and how they connect.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
public interface IReadOnlyGraph<TNode>
    where TNode : notnull
{
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
}
