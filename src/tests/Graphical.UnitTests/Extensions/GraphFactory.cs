namespace Graphical.UnitTests;

internal static class GraphFactory
{
    public static IGraph<TNode> Create<TNode>(Type openGraphType, IEqualityComparer<TNode>? comparer = null)
        where TNode : notnull
    {
        var graphType = openGraphType.MakeGenericType(typeof(TNode));
        return (IGraph<TNode>)Activator.CreateInstance(graphType, [comparer])!;
    }
}
