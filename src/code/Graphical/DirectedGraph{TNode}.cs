using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Graphical;

/// <summary>A graph whose edges point from a source node to a target node. Cycles and self-loops are allowed.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
[DebuggerDisplay("NodeCount = {NodeCount}, EdgeCount = {EdgeCount}")]
public class DirectedGraph<TNode> : Graph<TNode>, IDirectedGraph<TNode>
    where TNode : notnull
{
    private HashSet<int>[] _predecessors;

    // RS0022 flags these constructors because they make the non-inheritable Graph<TNode> inheritable through this
    // class. That is intended by the design: Graph<TNode> exposes no protected members (its hooks are private
    // protected), so deriving from DirectedGraph<TNode> outside this assembly cannot reach anything new.
#pragma warning disable RS0022
    /// <summary>Creates an empty graph that compares nodes with <see cref="EqualityComparer{T}.Default"/>.</summary>
    public DirectedGraph()
        : this(0, null)
    {
    }

    /// <summary>Creates an empty graph with room for <paramref name="nodeCapacity"/> nodes before it grows.</summary>
    /// <param name="nodeCapacity">The number of nodes the graph can hold before it grows.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nodeCapacity"/> is negative.</exception>
    public DirectedGraph(int nodeCapacity)
        : this(nodeCapacity, null)
    {
    }

    /// <summary>Creates an empty graph that compares nodes with <paramref name="comparer"/>.</summary>
    /// <param name="comparer">The node comparer, or <see langword="null"/> for <see cref="EqualityComparer{T}.Default"/>.</param>
    public DirectedGraph(IEqualityComparer<TNode>? comparer)
        : this(0, comparer)
    {
    }

    /// <summary>Creates an empty graph with the given initial capacity and node comparer.</summary>
    /// <param name="nodeCapacity">The number of nodes the graph can hold before it grows.</param>
    /// <param name="comparer">The node comparer, or <see langword="null"/> for <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nodeCapacity"/> is negative.</exception>
    public DirectedGraph(int nodeCapacity, IEqualityComparer<TNode>? comparer)
        : base(isDirected: true, nodeCapacity, comparer)
    {
        _predecessors = new HashSet<int>[nodeCapacity];
    }
#pragma warning restore RS0022

    /// <inheritdoc/>
    public IReadOnlyCollection<TNode> GetSuccessors(TNode node)
    {
        return new SlotSetView<TNode>(NodeTable, () => Adjacency, GetSlot(node));
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<TNode> GetPredecessors(TNode node)
    {
        return new SlotSetView<TNode>(NodeTable, () => _predecessors, GetSlot(node));
    }

    /// <inheritdoc/>
    public int GetInDegree(TNode node)
    {
        return _predecessors[GetSlot(node)].Count;
    }

    /// <inheritdoc/>
    public int GetOutDegree(TNode node)
    {
        return Adjacency[GetSlot(node)].Count;
    }

    private protected override void RemoveNodeCore(int slot)
    {
        var successors = Adjacency[slot];
        var predecessors = _predecessors[slot];
        foreach (var successor in successors)
        {
            _predecessors[successor].Remove(slot);
        }

        foreach (var predecessor in predecessors)
        {
            Adjacency[predecessor].Remove(slot);
        }

        var selfLoop = successors.Contains(slot) ? 1 : 0;
        EdgeCount -= successors.Count + predecessors.Count - selfLoop;
        successors.Clear();
        predecessors.Clear();
        base.RemoveNodeCore(slot);
    }

    private protected override void AddEdgeCore(int source, int target)
    {
        Adjacency[source].Add(target);
        _predecessors[target].Add(source);
        EdgeCount++;
    }

    private protected override void RemoveEdgeCore(int source, int target)
    {
        Adjacency[source].Remove(target);
        _predecessors[target].Remove(source);
        EdgeCount--;
    }

    private protected override IReadOnlyCollection<TNode> GetNeighborsCore(int slot)
    {
        return new SlotSetView<TNode>(NodeTable, () => Adjacency, () => _predecessors, slot);
    }

    private protected override int GetDegreeCore(int slot)
    {
        return Adjacency[slot].Count + _predecessors[slot].Count;
    }

    private protected override IEnumerable<Edge<TNode>> EnumerateEdgesCore()
    {
        for (var slot = 0; slot < NodeTable.SlotLimit; slot++)
        {
            if (!NodeTable.IsOccupied(slot))
            {
                continue;
            }

            foreach (var successor in Adjacency[slot])
            {
                yield return new Edge<TNode>(NodeTable[slot], NodeTable[successor]);
            }
        }
    }

    private protected override void ClearCore()
    {
        for (var slot = 0; slot < NodeTable.SlotLimit; slot++)
        {
            _predecessors[slot]?.Clear();
        }

        base.ClearCore();
    }

    private protected override void OnCapacityChanged(int capacity)
    {
        base.OnCapacityChanged(capacity);
        Array.Resize(ref _predecessors, capacity);
    }

    private protected override void OnNodeAdded(int slot)
    {
        base.OnNodeAdded(slot);
        _predecessors[slot] ??= [];
    }
}
