namespace Graphical;

/// <summary>A graph whose nodes and edges can be changed.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
public interface IGraph<TNode> : IReadOnlyGraph<TNode>
    where TNode : notnull
{
    /// <summary>Adds <paramref name="node"/> to the graph.</summary>
    /// <param name="node">The node to add.</param>
    /// <returns><see langword="true"/> if the node was added; <see langword="false"/> if it was already present.</returns>
    bool AddNode(TNode node);
}
