using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Graphical;

/// <summary>A graph whose edges have no direction.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
/// <remarks>
/// <para>
/// An edge joins two nodes symmetrically: <c>ContainsEdge(a, b)</c> equals <c>ContainsEdge(b, a)</c>, and adding
/// the reverse of an existing edge is a duplicate. Self-loops are allowed; one counts once in
/// <see cref="Graph{TNode}.EdgeCount"/> and adds 2 to the node's degree.
/// </para>
/// <para>
/// Not thread-safe for mutation: concurrent reads with no writer are safe, but any write requires exclusive access.
/// </para>
/// </remarks>
[DebuggerDisplay("NodeCount = {NodeCount}, EdgeCount = {EdgeCount}")]
public sealed class UndirectedGraph<TNode> : Graph<TNode>, IUndirectedGraph<TNode>
    where TNode : notnull
{
    /// <summary>Creates an empty graph that compares nodes with <see cref="EqualityComparer{T}.Default"/>.</summary>
    /// <remarks>O(1).</remarks>
    public UndirectedGraph()
        : this(0, null)
    {
    }

    /// <summary>Creates an empty graph with room for <paramref name="nodeCapacity"/> nodes before it grows.</summary>
    /// <param name="nodeCapacity">The number of nodes the graph can hold before it grows.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nodeCapacity"/> is negative.</exception>
    /// <remarks>O(<paramref name="nodeCapacity"/>).</remarks>
    public UndirectedGraph(int nodeCapacity)
        : this(nodeCapacity, null)
    {
    }

    /// <summary>Creates an empty graph that compares nodes with <paramref name="comparer"/>.</summary>
    /// <param name="comparer">The node comparer, or <see langword="null"/> for <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <remarks>O(1).</remarks>
    public UndirectedGraph(IEqualityComparer<TNode>? comparer)
        : this(0, comparer)
    {
    }

    /// <summary>Creates an empty graph with the given initial capacity and node comparer.</summary>
    /// <param name="nodeCapacity">The number of nodes the graph can hold before it grows.</param>
    /// <param name="comparer">The node comparer, or <see langword="null"/> for <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nodeCapacity"/> is negative.</exception>
    /// <remarks>O(<paramref name="nodeCapacity"/>).</remarks>
    public UndirectedGraph(int nodeCapacity, IEqualityComparer<TNode>? comparer)
        : base(isDirected: false, nodeCapacity, comparer)
    {
    }

    /// <inheritdoc/>
    public bool AreConnected(TNode first, TNode second)
    {
        ThrowHelper.ThrowIfNull(first);
        ThrowHelper.ThrowIfNull(second);
        if (!NodeTable.TryGetSlot(first, out var start) || !NodeTable.TryGetSlot(second, out var goal))
        {
            return false;
        }

        if (start == goal)
        {
            return true;
        }

        var visited = new BitSet(NodeTable.SlotLimit);
        var queue = new Queue<int>();
        visited.Set(start);
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            foreach (var neighbor in Adjacency[queue.Dequeue()])
            {
                if (neighbor == goal)
                {
                    return true;
                }

                if (!visited.Get(neighbor))
                {
                    visited.Set(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return false;
    }

    private protected override void RemoveNodeCore(int slot)
    {
        var neighbors = Adjacency[slot];
        foreach (var neighbor in neighbors)
        {
            if (neighbor != slot)
            {
                Adjacency[neighbor].Remove(slot);
            }
        }

        EdgeCount -= neighbors.Count;
        neighbors.Clear();
        base.RemoveNodeCore(slot);
    }

    private protected override void AddEdgeCore(int source, int target)
    {
        Adjacency[source].Add(target);
        Adjacency[target].Add(source);
        EdgeCount++;
    }

    private protected override void RemoveEdgeCore(int source, int target)
    {
        Adjacency[source].Remove(target);
        Adjacency[target].Remove(source);
        EdgeCount--;
    }

    private protected override IReadOnlyCollection<TNode> GetNeighborsCore(int slot)
    {
        return new SlotSetView<TNode>(NodeTable, () => Adjacency, slot);
    }

    /// <remarks>A self-loop contributes 2: it leaves and re-enters the node.</remarks>
    private protected override int GetDegreeCore(int slot)
    {
        var neighbors = Adjacency[slot];
        return neighbors.Contains(slot) ? neighbors.Count + 1 : neighbors.Count;
    }

    /// <remarks>
    /// Each edge is stored in both endpoints' sets; it is reported once, from the endpoint with the earlier
    /// insertion sequence, so <see cref="Edge{TNode}.Source"/> is the endpoint that joined the graph first.
    /// </remarks>
    private protected override IEnumerable<Edge<TNode>> EnumerateEdgesCore()
    {
        for (var slot = 0; slot < NodeTable.SlotLimit; slot++)
        {
            if (!NodeTable.IsOccupied(slot))
            {
                continue;
            }

            var sequence = NodeTable.GetSequence(slot);
            foreach (var neighbor in Adjacency[slot])
            {
                if (sequence <= NodeTable.GetSequence(neighbor))
                {
                    yield return new Edge<TNode>(NodeTable[slot], NodeTable[neighbor]);
                }
            }
        }
    }
}
