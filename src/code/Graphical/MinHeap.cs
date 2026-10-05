using System;

namespace Graphical;

/// <summary>
/// A binary min-heap of node slots ordered by a <see cref="long"/> key (the node's insertion sequence).
/// </summary>
/// <remarks>Push and pop are O(log n); the heap grows by doubling.</remarks>
internal sealed class MinHeap
{
    private const int DEFAULT_CAPACITY = 4;

    private Entry[] _entries = new Entry[DEFAULT_CAPACITY];

    public int Count { get; private set; }

    public void Push(int value, long key)
    {
        if (Count == _entries.Length)
        {
            Array.Resize(ref _entries, _entries.Length * 2);
        }

        var index = Count++;
        var entry = new Entry(value, key);
        while (index > 0)
        {
            var parent = (index - 1) / 2;
            if (_entries[parent].Key <= key)
            {
                break;
            }

            _entries[index] = _entries[parent];
            index = parent;
        }

        _entries[index] = entry;
    }

    public bool TryPop(out int value)
    {
        if (Count == 0)
        {
            value = default;
            return false;
        }

        value = _entries[0].Value;
        var last = _entries[--Count];
        var index = 0;
        while (true)
        {
            var child = (index * 2) + 1;
            if (child >= Count)
            {
                break;
            }

            if (child + 1 < Count && _entries[child + 1].Key < _entries[child].Key)
            {
                child++;
            }

            if (last.Key <= _entries[child].Key)
            {
                break;
            }

            _entries[index] = _entries[child];
            index = child;
        }

        _entries[index] = last;
        return true;
    }

    public void Clear()
    {
        Count = 0;
    }

    private readonly struct Entry
    {
        public Entry(int value, long key)
        {
            Value = value;
            Key = key;
        }

        public int Value { get; }

        public long Key { get; }
    }
}
