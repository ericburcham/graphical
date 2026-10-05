using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
[TestFixture(typeof(DirectedAcyclicGraph<>))]
[TestFixture(typeof(ReachabilityDirectedAcyclicGraph<>))]
internal sealed class WhenAddingEdgesInBulk
{
    private readonly Type _graphType;

    private IGraph<string> _graph = null!;

    private int _added;

    public WhenAddingEdgesInBulk(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
        _graph.AddEdge("b", "c");
        _added = _graph.AddEdges([Edge.Create("a", "b"), Edge.Create("b", "c"), Edge.Create("a", "b"), Edge.Create("c", "d")]);
    }

    [Test]
    public void AddEdgesShouldReturnTheNumberActuallyAdded()
    {
        _added.Should().Be(2);
    }

    [Test]
    public void EdgeCountShouldIncludeOnlyDistinctEdges()
    {
        _graph.EdgeCount.Should().Be(3);
    }

    [Test]
    public void MissingEndpointsShouldBeAdded()
    {
        _graph.Nodes.Should().Equal("b", "c", "a", "d");
    }
}
