namespace Graphical;

/// <summary>A graph whose nodes and edges can be changed.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
public interface IGraph<TNode> : IReadOnlyGraph<TNode>
    where TNode : notnull
{
}
