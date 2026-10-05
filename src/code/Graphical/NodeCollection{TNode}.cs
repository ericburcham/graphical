using System;
using System.Collections;
using System.Collections.Generic;

namespace Graphical;

/// <summary>A live read-only view of the nodes in a <see cref="NodeTable{TNode}"/>, in slot order.</summary>
internal sealed class NodeCollection<TNode> : IReadOnlyCollection<TNode>
    where TNode : notnull
{
    private readonly NodeTable<TNode> _table;

    public NodeCollection(NodeTable<TNode> table)
    {
        _table = table;
    }

    public int Count => _table.Count;

    public IEnumerator<TNode> GetEnumerator()
    {
        var version = _table.Version;
        for (var slot = 0; slot < _table.SlotLimit; slot++)
        {
            ThrowHelper.ThrowIfVersionChanged(version, _table.Version);
            if (_table.IsOccupied(slot))
            {
                yield return _table[slot];
            }
        }

        ThrowHelper.ThrowIfVersionChanged(version, _table.Version);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
