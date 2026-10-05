namespace Graphical;

/// <summary>A graph whose edges point from a source node to a target node, and whose nodes and edges can be changed.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
public interface IDirectedGraph<TNode> : IGraph<TNode>, IReadOnlyDirectedGraph<TNode>
    where TNode : notnull
{
}
