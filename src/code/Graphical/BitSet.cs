using System;
using System.Collections.Generic;

namespace Graphical;

/// <summary>A fixed-capacity set of non-negative integers backed by a <see cref="ulong"/> array.</summary>
internal sealed class BitSet
{
    private const int BITS_PER_WORD = 64;

    private const ulong DE_BRUIJN_SEQUENCE = 0x03F79D71B4CB0A89UL;

    private static readonly int[] DE_BRUIJN_POSITIONS =
    [
        0, 1, 48, 2, 57, 49, 28, 3, 61, 58, 50, 42, 38, 29, 17, 4,
        62, 55, 59, 36, 53, 51, 43, 22, 45, 39, 33, 30, 24, 18, 12, 5,
        63, 47, 56, 27, 60, 41, 37, 16, 54, 35, 52, 21, 44, 32, 23, 11,
        46, 26, 40, 15, 34, 20, 31, 10, 25, 14, 19, 9, 13, 8, 7, 6,
    ];

    private ulong[] _words;

    public BitSet(int capacity)
    {
        _words = new ulong[WordsFor(capacity)];
    }

    public void EnsureCapacity(int capacity)
    {
        EnsureWords(WordsFor(capacity));
    }

    public void UnionWith(BitSet other)
    {
        EnsureWords(other._words.Length);
        for (var i = 0; i < other._words.Length; i++)
        {
            _words[i] |= other._words[i];
        }
    }

    public void CopyFrom(BitSet other)
    {
        EnsureWords(other._words.Length);
        Array.Copy(other._words, _words, other._words.Length);
        Array.Clear(_words, other._words.Length, _words.Length - other._words.Length);
    }

    public void ClearAll()
    {
        Array.Clear(_words, 0, _words.Length);
    }

    public int PopCount()
    {
        var count = 0;
        foreach (var word in _words)
        {
            count += PopCount(word);
        }

        return count;
    }

    public IEnumerable<int> EnumerateSetBits()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            var word = _words[i];
            while (word != 0)
            {
                var lowest = word & (~word + 1);
                yield return (i * BITS_PER_WORD) + TrailingZeroCount(lowest);
                word ^= lowest;
            }
        }
    }

    // SWAR population count: sums bits in 2-, 4- and 8-bit lanes, then adds the bytes with a multiply.
    private static int PopCount(ulong word)
    {
        word -= (word >> 1) & 0x5555555555555555UL;
        word = (word & 0x3333333333333333UL) + ((word >> 2) & 0x3333333333333333UL);
        word = (word + (word >> 4)) & 0x0F0F0F0F0F0F0F0FUL;
        return (int)((word * 0x0101010101010101UL) >> 56);
    }

    // Index of the single set bit in an isolated power of two, via a de Bruijn sequence lookup.
    private static int TrailingZeroCount(ulong isolatedBit)
    {
        return DE_BRUIJN_POSITIONS[(int)((isolatedBit * DE_BRUIJN_SEQUENCE) >> 58)];
    }

    private void EnsureWords(int words)
    {
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
