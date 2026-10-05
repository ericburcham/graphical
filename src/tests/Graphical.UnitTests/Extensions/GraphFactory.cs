using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Graphical.UnitTests;

internal static class GraphFactory
{
    public static IGraph<TNode> Create<TNode>(Type openGraphType)
        where TNode : notnull
    {
        return Construct<TNode>(openGraphType);
    }

    public static IGraph<TNode> Create<TNode>(Type openGraphType, IEqualityComparer<TNode>? comparer)
        where TNode : notnull
    {
        return Construct<TNode>(openGraphType, comparer);
    }

    public static IGraph<TNode> Create<TNode>(Type openGraphType, int nodeCapacity)
        where TNode : notnull
    {
        return Construct<TNode>(openGraphType, nodeCapacity);
    }

    public static IGraph<TNode> Create<TNode>(Type openGraphType, int nodeCapacity, IEqualityComparer<TNode>? comparer)
        where TNode : notnull
    {
        return Construct<TNode>(openGraphType, nodeCapacity, comparer);
    }

    public static IDirectedGraph<TNode> CreateDirected<TNode>(Type openGraphType)
        where TNode : notnull
    {
        return (IDirectedGraph<TNode>)Construct<TNode>(openGraphType);
    }

    public static IDirectedAcyclicGraph<TNode> CreateAcyclic<TNode>(Type openGraphType)
        where TNode : notnull
    {
        return (IDirectedAcyclicGraph<TNode>)Construct<TNode>(openGraphType);
    }

    private static IGraph<TNode> Construct<TNode>(Type openGraphType, params object?[] arguments)
        where TNode : notnull
    {
        var graphType = openGraphType.MakeGenericType(typeof(TNode));
        var parameterTypes = arguments.Length switch
        {
            0 => Type.EmptyTypes,
            1 when arguments[0] is int => [typeof(int)],
            1 => [typeof(IEqualityComparer<TNode>)],
            _ => new[] { typeof(int), typeof(IEqualityComparer<TNode>) },
        };
        var constructor = graphType.GetConstructor(parameterTypes)
            ?? throw new MissingMethodException(graphType.Name, ".ctor");
        try
        {
            return (IGraph<TNode>)constructor.Invoke(arguments);
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }
    }
}
