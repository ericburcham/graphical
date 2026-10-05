using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Graphical;

/// <summary>A directed graph that rejects any edge that would create a cycle, including self-loops.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
[DebuggerDisplay("NodeCount = {NodeCount}, EdgeCount = {EdgeCount}")]
public class DirectedAcyclicGraph<TNode> : DirectedGraph<TNode>, IDirectedAcyclicGraph<TNode>
    where TNode : notnull
{
    /// <summary>Creates an empty graph that compares nodes with <see cref="EqualityComparer{T}.Default"/>.</summary>
    public DirectedAcyclicGraph()
        : this(0, null)
    {
    }

    /// <summary>Creates an empty graph with room for <paramref name="nodeCapacity"/> nodes before it grows.</summary>
    /// <param name="nodeCapacity">The number of nodes the graph can hold before it grows.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nodeCapacity"/> is negative.</exception>
    public DirectedAcyclicGraph(int nodeCapacity)
        : this(nodeCapacity, null)
    {
    }

    /// <summary>Creates an empty graph that compares nodes with <paramref name="comparer"/>.</summary>
    /// <param name="comparer">The node comparer, or <see langword="null"/> for <see cref="EqualityComparer{T}.Default"/>.</param>
    public DirectedAcyclicGraph(IEqualityComparer<TNode>? comparer)
        : this(0, comparer)
    {
    }

    /// <summary>Creates an empty graph with the given initial capacity and node comparer.</summary>
    /// <param name="nodeCapacity">The number of nodes the graph can hold before it grows.</param>
    /// <param name="comparer">The node comparer, or <see langword="null"/> for <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nodeCapacity"/> is negative.</exception>
    public DirectedAcyclicGraph(int nodeCapacity, IEqualityComparer<TNode>? comparer)
        : base(nodeCapacity, comparer)
    {
    }

    /// <inheritdoc/>
    public bool WouldCreateCycle(TNode source, TNode target)
    {
        ThrowHelper.ThrowIfNull(source);
        ThrowHelper.ThrowIfNull(target);
        if (Comparer.Equals(source, target))
        {
            return true;
        }

        return NodeTable.TryGetSlot(source, out var sourceSlot)
            && NodeTable.TryGetSlot(target, out var targetSlot)
            && WouldCreateCycleCore(sourceSlot, targetSlot);
    }

    /// <summary>An edge closes a cycle when it is a self-loop or its target already reaches its source.</summary>
    private protected virtual bool WouldCreateCycleCore(int source, int target)
    {
        return source == target || HasPathCore(target, source);
    }

    private protected override void OnAddingEdge(TNode source, TNode target)
    {
        if (WouldCreateCycle(source, target))
        {
            ThrowHelper.ThrowCycle(source, target);
        }
    }
}
