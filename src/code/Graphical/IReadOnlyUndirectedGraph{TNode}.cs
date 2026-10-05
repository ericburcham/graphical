using System;

namespace Graphical;

/// <summary>A read-only view of a graph whose edges have no direction.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
public interface IReadOnlyUndirectedGraph<TNode> : IReadOnlyGraph<TNode>
    where TNode : notnull
{
    /// <summary>Determines whether <paramref name="first"/> and <paramref name="second"/> are in the same connected component.</summary>
    /// <param name="first">One node.</param>
    /// <param name="second">The other node.</param>
    /// <returns>
    /// <see langword="true"/> if a chain of edges joins the nodes, or if they are the same existing node;
    /// <see langword="false"/> otherwise, including when either node is missing.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is <see langword="null"/>.</exception>
    bool AreConnected(TNode first, TNode second);
}
