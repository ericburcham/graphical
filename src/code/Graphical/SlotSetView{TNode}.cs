using System;
using System.Collections;
using System.Collections.Generic;

namespace Graphical;

/// <summary>
/// A live read-only view of one node's slot set (neighbors, successors or predecessors), or of the union of two
/// such sets (a directed node's successors and predecessors), mapped back to nodes.
/// </summary>
/// <remarks>
/// The view is tied to the node, not the slot: if the node is removed, the view is empty from then on, even after
/// its slot is reused by another node.
/// </remarks>
internal sealed class SlotSetView<TNode> : IReadOnlyCollection<TNode>
    where TNode : notnull
{
    private readonly NodeTable<TNode> _table;

    private readonly Func<HashSet<int>[]> _sets;

    private readonly Func<HashSet<int>[]>? _otherSets;

    private readonly int _slot;

    private readonly long _sequence;

    public SlotSetView(NodeTable<TNode> table, Func<HashSet<int>[]> sets, int slot)
        : this(table, sets, null, slot)
    {
    }

    public SlotSetView(NodeTable<TNode> table, Func<HashSet<int>[]> sets, Func<HashSet<int>[]>? otherSets, int slot)
    {
        _table = table;
        _sets = sets;
        _otherSets = otherSets;
        _slot = slot;
        _sequence = table.GetSequence(slot);
    }

    public int Count
    {
        get
        {
            if (!IsCurrent)
            {
                return 0;
            }

            var set = _sets()[_slot];
            var count = set.Count;
            if (_otherSets is not null)
            {
                foreach (var slot in _otherSets()[_slot])
                {
                    if (!set.Contains(slot))
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }

    private bool IsCurrent => _table.GetSequence(_slot) == _sequence;

    public IEnumerator<TNode> GetEnumerator()
    {
        if (!IsCurrent)
        {
            yield break;
        }

        var version = _table.Version;
        var set = _sets()[_slot];
        foreach (var slot in set)
        {
            ThrowHelper.ThrowIfVersionChanged(version, _table.Version);
            yield return _table[slot];
        }

        if (_otherSets is not null)
        {
            foreach (var slot in _otherSets()[_slot])
            {
                ThrowHelper.ThrowIfVersionChanged(version, _table.Version);
                if (!set.Contains(slot))
                {
                    yield return _table[slot];
                }
            }
        }

        ThrowHelper.ThrowIfVersionChanged(version, _table.Version);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
