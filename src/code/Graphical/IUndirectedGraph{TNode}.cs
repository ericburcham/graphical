namespace Graphical;

/// <summary>A graph whose edges have no direction and whose nodes and edges can be changed.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
public interface IUndirectedGraph<TNode> : IGraph<TNode>, IReadOnlyUndirectedGraph<TNode>
    where TNode : notnull
{
}
