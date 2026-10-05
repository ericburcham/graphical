using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
        return TryGetEdgeSlots(source, target, out _, out _);
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<TNode> GetNeighbors(TNode node)
    {
        return GetNeighborsCore(GetSlot(node));
    }

    /// <inheritdoc/>
    public int GetDegree(TNode node)
    {
        return GetDegreeCore(GetSlot(node));
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
    public bool RemoveNode(TNode node)
    {
        ThrowHelper.ThrowIfNull(node);
        if (!NodeTable.TryGetSlot(node, out var slot))
        {
            return false;
        }

        RemoveNodeCore(slot);
        return true;
    }

    /// <inheritdoc/>
    public int AddNodes(IEnumerable<TNode> nodes)
    {
        ThrowHelper.ThrowIfNull(nodes);
        var batch = new List<TNode>(nodes);
        foreach (var node in batch)
        {
            if (node is null)
            {
                ThrowHelper.ThrowNullItem(nameof(nodes));
            }
        }

        var added = 0;
        foreach (var node in batch)
        {
            if (!NodeTable.TryGetSlot(node, out _))
            {
                AddNodeSlot(node);
                added++;
            }
        }

        return added;
    }

    /// <inheritdoc/>
    public bool AddEdge(TNode source, TNode target)
    {
        ThrowHelper.ThrowIfNull(source);
        ThrowHelper.ThrowIfNull(target);
        return AddValidatedEdge(source, target);
    }

    /// <inheritdoc/>
    public int AddEdges(IEnumerable<Edge<TNode>> edges)
    {
        ThrowHelper.ThrowIfNull(edges);
        var batch = new List<Edge<TNode>>(edges);
        foreach (var edge in batch)
        {
            if (edge.Source is null || edge.Target is null)
            {
                ThrowHelper.ThrowNullEndpoint(nameof(edges));
            }
        }

        return AddEdgesCore(batch);
    }

    /// <inheritdoc/>
    public bool RemoveEdge(TNode source, TNode target)
    {
        if (!TryGetEdgeSlots(source, target, out var sourceSlot, out var targetSlot))
        {
            return false;
        }

        RemoveEdgeCore(sourceSlot, targetSlot);
        NodeTable.IncrementVersion();
        return true;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ClearCore();
    }

    internal int Version => NodeTable.Version;

    internal IEnumerable<Edge<TNode>> EnumerateEdges()
    {
        return EnumerateEdgesCore();
    }

    /// <summary>Adds a batch of edges whose endpoints are known to be non-null.</summary>
    /// <returns>The number of edges actually added.</returns>
    private protected virtual int AddEdgesCore(IReadOnlyList<Edge<TNode>> edges)
    {
        var added = 0;
        foreach (var edge in edges)
        {
            if (AddValidatedEdge(edge.Source, edge.Target))
            {
                added++;
            }
        }

        return added;
    }

    /// <summary>Adds an edge between two existing slots that are not yet connected, and counts it.</summary>
    private protected abstract void AddEdgeCore(int source, int target);

    /// <summary>Removes the node in <paramref name="slot"/>; overrides detach its edges first, then call the base.</summary>
    private protected virtual void RemoveNodeCore(int slot)
    {
        NodeTable.Remove(slot);
    }

    /// <summary>Removes an existing edge between two slots, and uncounts it.</summary>
    private protected abstract void RemoveEdgeCore(int source, int target);

    private protected abstract IEnumerable<Edge<TNode>> EnumerateEdgesCore();

    private protected abstract IReadOnlyCollection<TNode> GetNeighborsCore(int slot);

    private protected abstract int GetDegreeCore(int slot);

    /// <summary>Maps a node to its slot, throwing if the node is null or missing.</summary>
    private protected int GetSlot(TNode node, [CallerArgumentExpression(nameof(node))] string? paramName = null)
    {
        ThrowHelper.ThrowIfNull(node, paramName);
        if (!NodeTable.TryGetSlot(node, out var slot))
        {
            ThrowHelper.ThrowNodeNotFound(node);
        }

        return slot;
    }

    /// <summary>Removes every node and edge; overrides reset their own per-slot storage, then call the base.</summary>
    private protected virtual void ClearCore()
    {
        for (var slot = 0; slot < NodeTable.SlotLimit; slot++)
        {
            Adjacency[slot]?.Clear();
        }

        EdgeCount = 0;
        NodeTable.Clear();
    }

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

    private bool AddValidatedEdge(TNode source, TNode target)
    {
        if (TryFindEdge(source, target, out _, out _))
        {
            return false;
        }

        AddEdgeCore(GetOrAddSlot(source), GetOrAddSlot(target));
        NodeTable.IncrementVersion();
        return true;
    }

    /// <summary>Validates both endpoints and finds their slots when both exist and are joined by an edge.</summary>
    private bool TryGetEdgeSlots(TNode source, TNode target, out int sourceSlot, out int targetSlot)
    {
        ThrowHelper.ThrowIfNull(source);
        ThrowHelper.ThrowIfNull(target);
        return TryFindEdge(source, target, out sourceSlot, out targetSlot);
    }

    /// <summary>Finds the slots of two non-null endpoints when both exist and are joined by an edge.</summary>
    private bool TryFindEdge(TNode source, TNode target, out int sourceSlot, out int targetSlot)
    {
        targetSlot = -1;
        return NodeTable.TryGetSlot(source, out sourceSlot)
            && NodeTable.TryGetSlot(target, out targetSlot)
            && Adjacency[sourceSlot].Contains(targetSlot);
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
