using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

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

    public static void ThrowIfNull<T>([NotNull] T value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }

    [DoesNotReturn]
    public static void ThrowNullItem(string paramName)
    {
        throw new ArgumentException("The collection must not contain a null node.", paramName);
    }

    [DoesNotReturn]
    public static void ThrowNullEndpoint(string paramName)
    {
        throw new ArgumentException("Every edge must have a non-null source and target.", paramName);
    }

    [DoesNotReturn]
    public static void ThrowNodeNotFound<TNode>(TNode node)
    {
        throw new KeyNotFoundException($"The node '{node}' is not in the graph.");
    }

    [DoesNotReturn]
    public static void ThrowCycle<TNode>(TNode source, TNode target)
    {
        throw new GraphCycleException($"Adding the edge ({source} -> {target}) would create a cycle.");
    }

    [DoesNotReturn]
    public static void ThrowNegativeCapacity(int nodeCapacity)
    {
        throw new ArgumentOutOfRangeException(nameof(nodeCapacity), nodeCapacity, "The node capacity must not be negative.");
    }
}
