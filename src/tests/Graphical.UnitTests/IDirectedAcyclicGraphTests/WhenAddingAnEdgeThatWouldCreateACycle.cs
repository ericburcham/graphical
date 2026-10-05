using AwesomeAssertions;

namespace Graphical.UnitTests.IDirectedAcyclicGraphTests;

[TestFixture(typeof(DirectedAcyclicGraph<>))]
internal sealed class WhenAddingAnEdgeThatWouldCreateACycle
{
    private const string NEW_NODE = "new";

    private readonly Type _graphType;

    private readonly Dictionary<string, Exception?> _exceptions = [];

    private IDirectedAcyclicGraph<string> _graph = null!;

    private Edge<string>[] _edgesBefore = [];

    private string[] _nodesBefore = [];

    public WhenAddingAnEdgeThatWouldCreateACycle(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.CreateAcyclic<string>(_graphType);
        _graph.AddEdges([Edge.Create("a", "b"), Edge.Create("b", "c"), Edge.Create("c", "d")]);
        _edgesBefore = [.. _graph.Edges];
        _nodesBefore = [.. _graph.Nodes];

        _exceptions["self-loop"] = Catch.Exception(() => _graph.AddEdge("a", "a"));
        _exceptions["self-loop on a new node"] = Catch.Exception(() => _graph.AddEdge(NEW_NODE, NEW_NODE));
        _exceptions["two-cycle"] = Catch.Exception(() => _graph.AddEdge("b", "a"));
        _exceptions["long cycle"] = Catch.Exception(() => _graph.AddEdge("d", "a"));
    }

    [TestCase("self-loop")]
    [TestCase("self-loop on a new node")]
    [TestCase("two-cycle")]
    [TestCase("long cycle")]
    public void AddEdgeShouldThrowGraphCycleException(string attempt)
    {
        _exceptions[attempt].Should().BeOfType<GraphCycleException>();
    }

    [Test]
    public void TheEdgesShouldBeUnchanged()
    {
        _graph.Edges.Should().Equal(_edgesBefore);
    }

    [Test]
    public void NoEndpointShouldBeAutoAdded()
    {
        _graph.Nodes.Should().Equal(_nodesBefore);
    }
}
