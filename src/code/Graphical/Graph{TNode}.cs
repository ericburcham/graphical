using System.Collections.Generic;

namespace Graphical;

/// <summary>The base class for every graph in this library: node storage, edge counting and live views.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
public abstract class Graph<TNode> : IGraph<TNode>
    where TNode : notnull
{
    private protected Graph(int nodeCapacity, IEqualityComparer<TNode>? comparer)
    {
        if (nodeCapacity < 0)
        {
            ThrowHelper.ThrowNegativeCapacity(nodeCapacity);
        }

        NodeTable = new NodeTable<TNode>(nodeCapacity, comparer ?? EqualityComparer<TNode>.Default);
        Nodes = new NodeCollection<TNode>(NodeTable);
        Edges = new EdgeCollection<TNode>(this);
    }

    /// <inheritdoc/>
    public IEqualityComparer<TNode> Comparer => NodeTable.Comparer;

    /// <inheritdoc/>
    public int NodeCount => NodeTable.Count;

    /// <inheritdoc/>
    public int EdgeCount { get; private protected set; }

    /// <inheritdoc/>
    public IReadOnlyCollection<TNode> Nodes { get; }

    /// <inheritdoc/>
    public IReadOnlyCollection<Edge<TNode>> Edges { get; }

    private protected NodeTable<TNode> NodeTable { get; }

    /// <inheritdoc/>
    public bool ContainsNode(TNode node)
    {
        return NodeTable.TryGetSlot(node, out _);
    }

    /// <inheritdoc/>
    public bool AddNode(TNode node)
    {
        if (NodeTable.TryGetSlot(node, out _))
        {
            return false;
        }

        NodeTable.Add(node);
        return true;
    }

    private protected abstract IEnumerable<Edge<TNode>> EnumerateEdgesCore();

    internal IEnumerable<Edge<TNode>> EnumerateEdges()
    {
        return EnumerateEdgesCore();
    }
}
