using System;
using System.Collections.Generic;

namespace Graphical;

/// <summary>
/// Maps each node to a dense integer slot so graph storage can use arrays indexed by slot.
/// </summary>
/// <typeparam name="TNode">The node type.</typeparam>
internal sealed class NodeTable<TNode>
    where TNode : notnull
{
    private const int MINIMUM_GROWTH = 4;

    private readonly Dictionary<TNode, int> _slotsByNode;

    private TNode[] _nodes;

    private long[] _sequences;

    private int _slotLimit;

    private long _nextSequence = 1;

    public NodeTable(int capacity, IEqualityComparer<TNode> comparer)
    {
        _slotsByNode = new Dictionary<TNode, int>(capacity, comparer);
        _nodes = new TNode[capacity];
        _sequences = new long[capacity];
    }

    public int Count => _slotsByNode.Count;

    public TNode this[int slot] => _nodes[slot];

    public bool TryGetSlot(TNode node, out int slot)
    {
        return _slotsByNode.TryGetValue(node, out slot);
    }

    public long GetSequence(int slot)
    {
        return _sequences[slot];
    }

    public int Add(TNode node)
    {
        if (_slotLimit == _nodes.Length)
        {
            var capacity = Math.Max(MINIMUM_GROWTH, _nodes.Length * 2);
            Array.Resize(ref _nodes, capacity);
            Array.Resize(ref _sequences, capacity);
        }

        var slot = _slotLimit++;
        _slotsByNode.Add(node, slot);
        _nodes[slot] = node;
        _sequences[slot] = _nextSequence++;
        return slot;
    }
}
