using System;
using System.Diagnostics.CodeAnalysis;

namespace Graphical;

/// <summary>Central place for the exceptions the graphs throw, so messages stay consistent.</summary>
internal static class ThrowHelper
{
    public static void ThrowIfVersionChanged(int expected, int actual)
    {
        if (expected != actual)
        {
            throw new InvalidOperationException("The graph was modified; enumeration operation may not execute.");
        }
    }

    [DoesNotReturn]
    public static void ThrowNegativeCapacity(int nodeCapacity)
    {
        throw new ArgumentOutOfRangeException(nameof(nodeCapacity), nodeCapacity, "The node capacity must not be negative.");
    }
}
