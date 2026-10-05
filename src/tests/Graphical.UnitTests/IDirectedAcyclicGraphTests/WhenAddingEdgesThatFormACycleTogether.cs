using AwesomeAssertions;

namespace Graphical.UnitTests.IDirectedAcyclicGraphTests;

[TestFixture(typeof(DirectedAcyclicGraph<>))]
[TestFixture(typeof(ReachabilityDirectedAcyclicGraph<>))]
internal sealed class WhenAddingEdgesThatFormACycleTogether
{
    private readonly Type _graphType;

    private readonly Dictionary<string, Exception?> _exceptions = [];

    private IDirectedAcyclicGraph<string> _graph = null!;

    private Edge<string>[] _edgesBefore = [];

    private string[] _nodesBefore = [];

    private int _addedValidBatch;

    private Edge<string>[] _edgesAfterValidBatch = [];

    public WhenAddingEdgesThatFormACycleTogether(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.CreateAcyclic<string>(_graphType);
        _graph.AddEdges([Edge.Create("a", "b"), Edge.Create("b", "c")]);
        _edgesBefore = [.. _graph.Edges];
        _nodesBefore = [.. _graph.Nodes];

        _exceptions["cycle among new nodes"] = Catch.Exception(
            () => _graph.AddEdges([Edge.Create("x", "y"), Edge.Create("y", "z"), Edge.Create("z", "x")]));
        _exceptions["cycle through existing nodes"] = Catch.Exception(
            () => _graph.AddEdges([Edge.Create("c", "d"), Edge.Create("d", "a")]));
        _exceptions["self-loop in a batch"] = Catch.Exception(
            () => _graph.AddEdges([Edge.Create("c", "d"), Edge.Create("q", "q")]));
        _exceptions["edge reversing an earlier batch edge"] = Catch.Exception(
            () => _graph.AddEdges([Edge.Create("p", "r"), Edge.Create("r", "p")]));

        var unchanged = _graph.Edges.SequenceEqual(_edgesBefore) && _graph.Nodes.SequenceEqual(_nodesBefore);
        _exceptions["graph changed"] = unchanged ? null : new InvalidOperationException("The graph changed.");

        _addedValidBatch = _graph.AddEdges([Edge.Create("c", "d"), Edge.Create("d", "e"), Edge.Create("a", "e"), Edge.Create("a", "b")]);
        _edgesAfterValidBatch = [.. _graph.Edges];
    }

    [TestCase("cycle among new nodes")]
    [TestCase("cycle through existing nodes")]
    [TestCase("self-loop in a batch")]
    [TestCase("edge reversing an earlier batch edge")]
    public void AddEdgesShouldThrowGraphCycleException(string attempt)
    {
        _exceptions[attempt].Should().BeOfType<GraphCycleException>();
    }

    [Test]
    public void TheGraphShouldBeExactlyAsItWasAfterEachRejection()
    {
        _exceptions["graph changed"].Should().BeNull();
    }

    [Test]
    public void AnAcyclicBatchShouldAddEveryNewEdge()
    {
        _addedValidBatch.Should().Be(3);
    }

    [Test]
    public void AnAcyclicBatchShouldLeaveTheGraphWithAllEdges()
    {
        _edgesAfterValidBatch.Should().HaveCount(5);
    }
}
