using System;
using System.Collections.Generic;

namespace Graphical;

/// <summary>A directed pair of nodes: an edge from <see cref="Source"/> to <see cref="Target"/>.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
/// <remarks>
/// Edge equality is ordered and always compares endpoints with <see cref="EqualityComparer{T}.Default"/>,
/// even when the edge came from a graph that uses a custom node comparer.
/// </remarks>
public readonly struct Edge<TNode> : IEquatable<Edge<TNode>>
    where TNode : notnull
{
    /// <summary>Creates an edge from <paramref name="source"/> to <paramref name="target"/>.</summary>
    /// <param name="source">The node the edge starts at.</param>
    /// <param name="target">The node the edge ends at.</param>
    public Edge(TNode source, TNode target)
    {
        Source = source;
        Target = target;
    }

    /// <summary>Gets the node the edge starts at.</summary>
    public TNode Source { get; }

    /// <summary>Gets the node the edge ends at.</summary>
    public TNode Target { get; }

    /// <summary>Returns the edge pointing the other way, from <see cref="Target"/> to <see cref="Source"/>.</summary>
    /// <returns>A new edge with the endpoints swapped.</returns>
    /// <remarks>O(1).</remarks>
    public Edge<TNode> Reverse()
    {
        return new Edge<TNode>(Target, Source);
    }

    /// <summary>Deconstructs the edge into its endpoints.</summary>
    /// <param name="source">Receives <see cref="Source"/>.</param>
    /// <param name="target">Receives <see cref="Target"/>.</param>
    /// <remarks>O(1).</remarks>
    public void Deconstruct(out TNode source, out TNode target)
    {
        source = Source;
        target = Target;
    }

    /// <summary>Determines whether this edge has the same source and target as <paramref name="other"/>.</summary>
    /// <param name="other">The edge to compare with.</param>
    /// <returns><see langword="true"/> if both endpoints are equal under <see cref="EqualityComparer{T}.Default"/>; otherwise <see langword="false"/>.</returns>
    /// <remarks>O(1) plus the cost of comparing two pairs of nodes.</remarks>
    public bool Equals(Edge<TNode> other)
    {
        var comparer = EqualityComparer<TNode>.Default;
        return comparer.Equals(Source, other.Source) && comparer.Equals(Target, other.Target);
    }

    /// <inheritdoc cref="Equals(Edge{TNode})"/>
    /// <param name="obj">The object to compare with.</param>
    public override bool Equals(object? obj)
    {
        return obj is Edge<TNode> other && Equals(other);
    }

    /// <summary>Returns a hash code combining both endpoints, consistent with <see cref="Equals(Edge{TNode})"/>.</summary>
    /// <returns>The hash code.</returns>
    /// <remarks>O(1) plus the cost of hashing two nodes.</remarks>
    public override int GetHashCode()
    {
        var comparer = EqualityComparer<TNode>.Default;
        unchecked
        {
            return (comparer.GetHashCode(Source) * -1521134295) + comparer.GetHashCode(Target);
        }
    }

    /// <summary>Determines whether two edges have the same source and target.</summary>
    /// <param name="left">The first edge.</param>
    /// <param name="right">The second edge.</param>
    /// <returns><see langword="true"/> if the edges are equal; otherwise <see langword="false"/>.</returns>
    public static bool operator ==(Edge<TNode> left, Edge<TNode> right)
    {
        return left.Equals(right);
    }

    /// <summary>Determines whether two edges differ in source or target.</summary>
    /// <param name="left">The first edge.</param>
    /// <param name="right">The second edge.</param>
    /// <returns><see langword="true"/> if the edges are not equal; otherwise <see langword="false"/>.</returns>
    public static bool operator !=(Edge<TNode> left, Edge<TNode> right)
    {
        return !left.Equals(right);
    }
}
