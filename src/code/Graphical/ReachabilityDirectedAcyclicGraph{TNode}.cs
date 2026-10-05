using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Graphical;

/// <summary>
/// A directed acyclic graph that maintains its transitive closure, so reachability queries are answered in O(1).
/// </summary>
/// <typeparam name="TNode">The node type.</typeparam>
[DebuggerDisplay("NodeCount = {NodeCount}, EdgeCount = {EdgeCount}")]
public sealed class ReachabilityDirectedAcyclicGraph<TNode> : DirectedAcyclicGraph<TNode>
    where TNode : notnull
{
    private const int INVARIANT_CHECK_SLOT_LIMIT = 256;

    private BitSet[] _descendants;

    private BitSet[] _ancestors;

    /// <summary>Creates an empty graph that compares nodes with <see cref="EqualityComparer{T}.Default"/>.</summary>
    public ReachabilityDirectedAcyclicGraph()
        : this(0, null)
    {
    }

    /// <summary>Creates an empty graph with room for <paramref name="nodeCapacity"/> nodes before it grows.</summary>
    /// <param name="nodeCapacity">The number of nodes the graph can hold before it grows.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nodeCapacity"/> is negative.</exception>
    public ReachabilityDirectedAcyclicGraph(int nodeCapacity)
        : this(nodeCapacity, null)
    {
    }

    /// <summary>Creates an empty graph that compares nodes with <paramref name="comparer"/>.</summary>
    /// <param name="comparer">The node comparer, or <see langword="null"/> for <see cref="EqualityComparer{T}.Default"/>.</param>
    public ReachabilityDirectedAcyclicGraph(IEqualityComparer<TNode>? comparer)
        : this(0, comparer)
    {
    }

    /// <summary>Creates an empty graph with the given initial capacity and node comparer.</summary>
    /// <param name="nodeCapacity">The number of nodes the graph can hold before it grows.</param>
    /// <param name="comparer">The node comparer, or <see langword="null"/> for <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nodeCapacity"/> is negative.</exception>
    public ReachabilityDirectedAcyclicGraph(int nodeCapacity, IEqualityComparer<TNode>? comparer)
        : base(nodeCapacity, comparer)
    {
        _descendants = new BitSet[nodeCapacity];
        _ancestors = new BitSet[nodeCapacity];
    }

    /// <summary>O(1): one bit lookup in the source's descendant set.</summary>
    private protected override bool HasPathCore(int source, int target)
    {
        return _descendants[source].Get(target);
    }

    /// <summary>O(1): a self-loop, or the target already reaches the source.</summary>
    private protected override bool WouldCreateCycleCore(int source, int target)
    {
        return source == target || _descendants[target].Get(source);
    }

    private protected override IReadOnlyCollection<TNode> GetAncestorsCore(int slot)
    {
        return ToNodes(_ancestors[slot]);
    }

    private protected override IReadOnlyCollection<TNode> GetDescendantsCore(int slot)
    {
        return ToNodes(_descendants[slot]);
    }

    /// <summary>
    /// Adding u → v changes nothing when v is already a descendant of u. Otherwise every node in
    /// A = {u} ∪ ancestors(u) gains D = {v} ∪ descendants(v) as descendants, and every node in D gains A as ancestors.
    /// </summary>
    private protected override void AddEdgeCore(int source, int target)
    {
        if (_descendants[source].Get(target))
        {
            base.AddEdgeCore(source, target);
            return;
        }

        var reachers = WithSlot(_ancestors[source], source);
        var reached = WithSlot(_descendants[target], target);
        base.AddEdgeCore(source, target);
        foreach (var ancestor in reachers.EnumerateSetBits())
        {
            _descendants[ancestor].UnionWith(reached);
        }

        foreach (var descendant in reached.EnumerateSetBits())
        {
            _ancestors[descendant].UnionWith(reachers);
        }

        AssertClosureInvariants();
    }

    /// <summary>
    /// Removing u → v can only shrink the descendant sets of A = {u} ∪ ancestors(u) and the ancestor sets of
    /// D = {v} ∪ descendants(v); those are recomputed from neighbors, reusing the topological order from before the
    /// removal (removing an edge never invalidates it).
    /// </summary>
    private protected override void RemoveEdgeCore(int source, int target)
    {
        var reachers = WithSlot(_ancestors[source], source);
        var reached = WithSlot(_descendants[target], target);
        var order = GetTopologicalSlots();
        base.RemoveEdgeCore(source, target);
        Recompute(order, reachers, reached);
        AssertClosureInvariants();
    }

    /// <summary>
    /// Removing x can only shrink the descendant sets of its ancestors and the ancestor sets of its descendants;
    /// both groups are recomputed in one pass rather than once per incident edge.
    /// </summary>
    private protected override void RemoveNodeCore(int slot)
    {
        var reachers = Copy(_ancestors[slot]);
        var reached = Copy(_descendants[slot]);
        var order = GetTopologicalSlots();
        base.RemoveNodeCore(slot);
        _descendants[slot].ClearAll();
        _ancestors[slot].ClearAll();
        Recompute(order, reachers, reached);
        AssertClosureInvariants();
    }

    private protected override void ClearCore()
    {
        for (var slot = 0; slot < NodeTable.SlotLimit; slot++)
        {
            _descendants[slot]?.ClearAll();
            _ancestors[slot]?.ClearAll();
        }

        base.ClearCore();
    }

    /// <summary>Grows the bit arrays: each bit set must hold one bit per slot.</summary>
    private protected override void OnCapacityChanged(int capacity)
    {
        base.OnCapacityChanged(capacity);
        Array.Resize(ref _descendants, capacity);
        Array.Resize(ref _ancestors, capacity);
        foreach (var bits in _descendants)
        {
            bits?.EnsureCapacity(capacity);
        }

        foreach (var bits in _ancestors)
        {
            bits?.EnsureCapacity(capacity);
        }
    }

    private protected override void OnNodeAdded(int slot)
    {
        base.OnNodeAdded(slot);
        _descendants[slot] ??= new BitSet(NodeTable.Capacity);
        _ancestors[slot] ??= new BitSet(NodeTable.Capacity);
    }

    /// <summary>
    /// Recomputes the descendant sets of <paramref name="reachers"/> in reverse topological order, and the ancestor
    /// sets of <paramref name="reached"/> in topological order, so every set is rebuilt from sets already correct.
    /// </summary>
    private void Recompute(int[] order, BitSet reachers, BitSet reached)
    {
        for (var index = order.Length - 1; index >= 0; index--)
        {
            var slot = order[index];
            if (reachers.Get(slot) && NodeTable.IsOccupied(slot))
            {
                RebuildFrom(_descendants, slot, Adjacency[slot]);
            }
        }

        foreach (var slot in order)
        {
            if (reached.Get(slot) && NodeTable.IsOccupied(slot))
            {
                RebuildFrom(_ancestors, slot, Predecessors[slot]);
            }
        }
    }

    /// <summary>Sets <c>closure[slot]</c> to the union over <paramref name="neighbors"/> n of {n} ∪ closure[n].</summary>
    private static void RebuildFrom(BitSet[] closure, int slot, HashSet<int> neighbors)
    {
        var bits = closure[slot];
        bits.ClearAll();
        foreach (var neighbor in neighbors)
        {
            bits.Set(neighbor);
            bits.UnionWith(closure[neighbor]);
        }
    }

    private BitSet Copy(BitSet bits)
    {
        var copy = new BitSet(NodeTable.Capacity);
        copy.CopyFrom(bits);
        return copy;
    }

    private BitSet WithSlot(BitSet bits, int slot)
    {
        var copy = Copy(bits);
        copy.Set(slot);
        return copy;
    }

    /// <summary>
    /// Checks the closure invariants in DEBUG builds: descendants and ancestors mirror each other, free slots have no
    /// bits, and no node is its own ancestor. Skipped above <see cref="INVARIANT_CHECK_SLOT_LIMIT"/> slots because
    /// the check is quadratic.
    /// </summary>
    [Conditional("DEBUG")]
    private void AssertClosureInvariants()
    {
        if (NodeTable.SlotLimit > INVARIANT_CHECK_SLOT_LIMIT)
        {
            return;
        }

        for (var slot = 0; slot < NodeTable.SlotLimit; slot++)
        {
            var occupied = NodeTable.IsOccupied(slot);
            Debug.Assert(occupied || _descendants[slot] is null || _descendants[slot].PopCount() == 0, "A free slot has descendants.");
            Debug.Assert(occupied || _ancestors[slot] is null || _ancestors[slot].PopCount() == 0, "A free slot has ancestors.");
            if (!occupied)
            {
                continue;
            }

            Debug.Assert(!_ancestors[slot].Get(slot), "A node is its own ancestor.");
            foreach (var descendant in _descendants[slot].EnumerateSetBits())
            {
                Debug.Assert(NodeTable.IsOccupied(descendant), "A descendant is a free slot.");
                Debug.Assert(_ancestors[descendant].Get(slot), "Descendants and ancestors disagree.");
            }

            foreach (var ancestor in _ancestors[slot].EnumerateSetBits())
            {
                Debug.Assert(NodeTable.IsOccupied(ancestor), "An ancestor is a free slot.");
                Debug.Assert(_descendants[ancestor].Get(slot), "Ancestors and descendants disagree.");
            }
        }
    }
}
