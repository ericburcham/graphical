using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
[TestFixture(typeof(DirectedAcyclicGraph<>))]
[TestFixture(typeof(ReachabilityDirectedAcyclicGraph<>))]
internal sealed class WhenMutatingTheGraphDuringEnumeration
{
    private const string HUB = "hub";

    private readonly Type _graphType;

    private readonly Dictionary<string, Exception?> _exceptions = [];

    public WhenMutatingTheGraphDuringEnumeration(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _exceptions["Nodes"] = EnumerateWhileMutating(graph => graph.Nodes, graph => graph.AddNode("new"));
        _exceptions["Edges"] = EnumerateWhileMutating(graph => graph.Edges, graph => graph.AddEdge("x", "y"));
        _exceptions["GetNeighbors"] = EnumerateWhileMutating(graph => graph.GetNeighbors(HUB), graph => graph.RemoveEdge("a", "b"));
        _exceptions["Nodes after removal"] = EnumerateWhileMutating(graph => graph.Nodes, graph => graph.RemoveNode("b"));
    }

    [TestCase("Nodes")]
    [TestCase("Edges")]
    [TestCase("GetNeighbors")]
    [TestCase("Nodes after removal")]
    public void ContinuingTheEnumerationShouldThrowInvalidOperationException(string view)
    {
        _exceptions[view].Should().BeOfType<InvalidOperationException>();
    }

    private Exception? EnumerateWhileMutating<T>(Func<IGraph<string>, IEnumerable<T>> view, Action<IGraph<string>> mutate)
    {
        var graph = GraphFactory.Create<string>(_graphType);
        graph.AddEdges([Edge.Create(HUB, "a"), Edge.Create(HUB, "b"), Edge.Create("a", "b")]);
        using var enumerator = view(graph).GetEnumerator();
        enumerator.MoveNext();
        mutate(graph);
        return Catch.Exception(() => enumerator.MoveNext());
    }
}
