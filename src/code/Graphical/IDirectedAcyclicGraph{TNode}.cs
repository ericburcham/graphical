namespace Graphical;

/// <summary>A directed graph that has no cycles and whose nodes and edges can be changed.</summary>
/// <typeparam name="TNode">The node type.</typeparam>
public interface IDirectedAcyclicGraph<TNode> : IDirectedGraph<TNode>, IReadOnlyDirectedAcyclicGraph<TNode>
    where TNode : notnull
{
}
