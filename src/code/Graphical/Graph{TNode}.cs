using System;
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
        Adjacency = new HashSet<int>[nodeCapacity];
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

    /// <summary>Gets the per-slot adjacency sets: neighbors when undirected, successors when directed.</summary>
    private protected HashSet<int>[] Adjacency { get; private set; }

    /// <inheritdoc/>
    public bool ContainsNode(TNode node)
    {
        ThrowHelper.ThrowIfNull(node);
        return NodeTable.TryGetSlot(node, out _);
    }

    /// <inheritdoc/>
    public bool ContainsEdge(TNode source, TNode target)
    {
        ThrowHelper.ThrowIfNull(source);
        ThrowHelper.ThrowIfNull(target);
        return NodeTable.TryGetSlot(source, out var sourceSlot)
            && NodeTable.TryGetSlot(target, out var targetSlot)
            && Adjacency[sourceSlot].Contains(targetSlot);
    }

    /// <inheritdoc/>
    public bool AddNode(TNode node)
    {
        ThrowHelper.ThrowIfNull(node);
        if (NodeTable.TryGetSlot(node, out _))
        {
            return false;
        }

        AddNodeSlot(node);
        return true;
    }

    /// <inheritdoc/>
    public bool AddEdge(TNode source, TNode target)
    {
        ThrowHelper.ThrowIfNull(source);
        ThrowHelper.ThrowIfNull(target);
        if (ContainsEdge(source, target))
        {
            return false;
        }

        AddEdgeCore(GetOrAddSlot(source), GetOrAddSlot(target));
        NodeTable.IncrementVersion();
        return true;
    }

    internal int Version => NodeTable.Version;

    internal IEnumerable<Edge<TNode>> EnumerateEdges()
    {
        return EnumerateEdgesCore();
    }

    /// <summary>Adds an edge between two existing slots that are not yet connected, and counts it.</summary>
    private protected abstract void AddEdgeCore(int source, int target);

    private protected abstract IEnumerable<Edge<TNode>> EnumerateEdgesCore();

    /// <summary>Grows every per-slot array to <paramref name="capacity"/>.</summary>
    private protected virtual void OnCapacityChanged(int capacity)
    {
        var adjacency = Adjacency;
        Array.Resize(ref adjacency, capacity);
        Adjacency = adjacency;
    }

    /// <summary>Prepares per-slot storage for a node that was just given <paramref name="slot"/>.</summary>
    private protected virtual void OnNodeAdded(int slot)
    {
        Adjacency[slot] ??= [];
    }

    private int GetOrAddSlot(TNode node)
    {
        return NodeTable.TryGetSlot(node, out var slot) ? slot : AddNodeSlot(node);
    }

    private int AddNodeSlot(TNode node)
    {
        var capacity = NodeTable.Capacity;
        var slot = NodeTable.Add(node);
        if (NodeTable.Capacity != capacity)
        {
            OnCapacityChanged(NodeTable.Capacity);
        }

        OnNodeAdded(slot);
        return slot;
    }
}
