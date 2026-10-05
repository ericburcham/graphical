using System;
using System.Collections.Generic;

namespace Graphical;

/// <summary>
/// Maps each node to a dense integer slot so graph storage can use arrays indexed by slot.
/// </summary>
/// <remarks>
/// Freed slots are reused (last freed, first reused). Each slot records a monotonically increasing insertion
/// sequence, so callers can order nodes by when they were added even after slots are reused. A version counter
/// changes on every mutation so live views can detect modification during enumeration.
/// </remarks>
/// <typeparam name="TNode">The node type.</typeparam>
internal sealed class NodeTable<TNode>
    where TNode : notnull
{
    private const int MINIMUM_GROWTH = 4;

    private const long FREE = 0;

    private readonly Dictionary<TNode, int> _slotsByNode;

    private readonly Stack<int> _freeSlots = new();

    private TNode[] _nodes;

    private long[] _sequences;

    private long _nextSequence = FREE + 1;

    public NodeTable(int capacity, IEqualityComparer<TNode> comparer)
    {
        _slotsByNode = new Dictionary<TNode, int>(capacity, comparer);
        _nodes = new TNode[capacity];
        _sequences = new long[capacity];
    }

    public IEqualityComparer<TNode> Comparer => _slotsByNode.Comparer;

    public int Count => _slotsByNode.Count;

    /// <summary>Gets the length of the slot-indexed arrays; per-slot storage must be at least this long.</summary>
    public int Capacity => _nodes.Length;

    /// <summary>Gets one past the highest slot ever handed out since the last clear.</summary>
    public int SlotLimit { get; private set; }

    public int Version { get; private set; }

    public TNode this[int slot] => _nodes[slot];

    public bool TryGetSlot(TNode node, out int slot)
    {
        return _slotsByNode.TryGetValue(node, out slot);
    }

    public bool IsOccupied(int slot)
    {
        return _sequences[slot] != FREE;
    }

    public long GetSequence(int slot)
    {
        return _sequences[slot];
    }

    /// <summary>Adds a node that is not already present and returns its slot.</summary>
    public int Add(TNode node)
    {
        var slot = _freeSlots.Count > 0 ? _freeSlots.Pop() : TakeNewSlot();
        _slotsByNode.Add(node, slot);
        _nodes[slot] = node;
        _sequences[slot] = _nextSequence++;
        Version++;
        return slot;
    }

    public void Remove(int slot)
    {
        _slotsByNode.Remove(_nodes[slot]);
        _nodes[slot] = default!;
        _sequences[slot] = FREE;
        _freeSlots.Push(slot);
        Version++;
    }

    public void Clear()
    {
        _slotsByNode.Clear();
        _freeSlots.Clear();
        Array.Clear(_nodes, 0, SlotLimit);
        Array.Clear(_sequences, 0, SlotLimit);
        SlotLimit = 0;
        Version++;
    }

    /// <summary>Records a mutation that does not change the node set, such as adding or removing an edge.</summary>
    public void IncrementVersion()
    {
        Version++;
    }

    private int TakeNewSlot()
    {
        if (SlotLimit == _nodes.Length)
        {
            var capacity = Math.Max(MINIMUM_GROWTH, _nodes.Length * 2);
            Array.Resize(ref _nodes, capacity);
            Array.Resize(ref _sequences, capacity);
        }

        return SlotLimit++;
    }
}
