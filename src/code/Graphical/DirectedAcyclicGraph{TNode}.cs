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

    /// <summary>Adds the batch only if the graph plus every new edge in it stays acyclic.</summary>
    private protected override int AddEdgesCore(IReadOnlyList<Edge<TNode>> edges)
    {
        ThrowIfBatchCreatesCycle(edges);
        return AddEdgesWithoutChecks(edges, checkEachEdge: false);
    }

    private protected override void OnAddingEdge(TNode source, TNode target)
    {
        if (WouldCreateCycle(source, target))
        {
            ThrowHelper.ThrowCycle(source, target);
        }
    }

    /// <summary>
    /// Runs Kahn's algorithm over the current graph plus the batch's new edges, giving missing endpoints provisional
    /// indices past <see cref="NodeTable{TNode}.SlotLimit"/>. The combined graph is acyclic exactly when every node is
    /// dequeued. O(V + E + k) for a batch of k edges; nothing is changed.
    /// </summary>
    private void ThrowIfBatchCreatesCycle(IReadOnlyList<Edge<TNode>> edges)
    {
        var slotLimit = NodeTable.SlotLimit;
        var provisional = new Dictionary<TNode, int>(Comparer);
        var newEdges = new HashSet<(int Source, int Target)>();
        foreach (var edge in edges)
        {
            var source = IndexOf(edge.Source);
            var target = IndexOf(edge.Target);
            if (source == target)
            {
                ThrowHelper.ThrowCycle(edge.Source, edge.Target);
            }

            if (source >= slotLimit || target >= slotLimit || !Adjacency[source].Contains(target))
            {
                newEdges.Add((source, target));
            }
        }

        if (newEdges.Count == 0)
        {
            return;
        }

        var total = slotLimit + provisional.Count;
        var inDegrees = new int[total];
        var extraSuccessors = new List<int>?[total];
        for (var slot = 0; slot < slotLimit; slot++)
        {
            if (NodeTable.IsOccupied(slot))
            {
                inDegrees[slot] = Predecessors[slot].Count;
            }
        }

        foreach (var (source, target) in newEdges)
        {
            inDegrees[target]++;
            (extraSuccessors[source] ??= []).Add(target);
        }

        var ready = new Queue<int>();
        for (var index = 0; index < total; index++)
        {
            if ((index >= slotLimit || NodeTable.IsOccupied(index)) && inDegrees[index] == 0)
            {
                ready.Enqueue(index);
            }
        }

        var processed = 0;
        while (ready.Count > 0)
        {
            var index = ready.Dequeue();
            processed++;
            if (index < slotLimit)
            {
                foreach (var successor in Adjacency[index])
                {
                    if (--inDegrees[successor] == 0)
                    {
                        ready.Enqueue(successor);
                    }
                }
            }

            foreach (var successor in extraSuccessors[index] ?? [])
            {
                if (--inDegrees[successor] == 0)
                {
                    ready.Enqueue(successor);
                }
            }
        }

        if (processed < NodeCount + provisional.Count)
        {
            throw new GraphCycleException("Adding the edges would create a cycle.");
        }

        int IndexOf(TNode node)
        {
            if (NodeTable.TryGetSlot(node, out var slot))
            {
                return slot;
            }

            if (!provisional.TryGetValue(node, out var index))
            {
                index = slotLimit + provisional.Count;
                provisional.Add(node, index);
            }

            return index;
        }
    }
}
