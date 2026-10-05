using System;

namespace Graphical;

/// <summary>A fixed-capacity set of non-negative integers backed by a <see cref="ulong"/> array.</summary>
internal sealed class BitSet
{
    private const int BITS_PER_WORD = 64;

    private ulong[] _words;

    public BitSet(int capacity)
    {
        _words = new ulong[WordsFor(capacity)];
    }

    public void EnsureCapacity(int capacity)
    {
        var words = WordsFor(capacity);
        if (words > _words.Length)
        {
            Array.Resize(ref _words, words);
        }
    }

    private static int WordsFor(int capacity)
    {
        return (capacity + BITS_PER_WORD - 1) / BITS_PER_WORD;
    }

    public bool Get(int index)
    {
        return (_words[index / BITS_PER_WORD] & (1UL << (index % BITS_PER_WORD))) != 0;
    }

    public void Set(int index)
    {
        _words[index / BITS_PER_WORD] |= 1UL << (index % BITS_PER_WORD);
    }

    public void Clear(int index)
    {
        _words[index / BITS_PER_WORD] &= ~(1UL << (index % BITS_PER_WORD));
    }
}
