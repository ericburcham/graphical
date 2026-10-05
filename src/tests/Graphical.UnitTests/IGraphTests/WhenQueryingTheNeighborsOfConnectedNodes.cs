using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
[TestFixture(typeof(DirectedAcyclicGraph<>))]
internal sealed class WhenQueryingTheNeighborsOfConnectedNodes
{
    private const string HUB = "hub";

    private const string FIRST = "first";

    private const string SECOND = "second";

    private const string ISOLATED = "isolated";

    private readonly Type _graphType;

    private IGraph<string> _graph = null!;

    public WhenQueryingTheNeighborsOfConnectedNodes(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
        _graph.AddEdge(HUB, FIRST);
        _graph.AddEdge(SECOND, HUB);
        _graph.AddNode(ISOLATED);
    }

    [Test]
    public void TheHubShouldNeighborBothEndpoints()
    {
        _graph.GetNeighbors(HUB).Should().BeEquivalentTo([FIRST, SECOND]);
    }

    [Test]
    public void AnEdgeTargetShouldNeighborItsSource()
    {
        _graph.GetNeighbors(FIRST).Should().Equal(HUB);
    }

    [Test]
    public void AnEdgeSourceShouldNeighborItsTarget()
    {
        _graph.GetNeighbors(SECOND).Should().Equal(HUB);
    }

    [Test]
    public void AnIsolatedNodeShouldHaveNoNeighbors()
    {
        _graph.GetNeighbors(ISOLATED).Should().BeEmpty();
    }

    [Test]
    public void TheHubDegreeShouldCountBothEdges()
    {
        _graph.GetDegree(HUB).Should().Be(2);
    }

    [Test]
    public void AnEndpointDegreeShouldBeOne()
    {
        _graph.GetDegree(FIRST).Should().Be(1);
    }

    [Test]
    public void AnIsolatedNodeDegreeShouldBeZero()
    {
        _graph.GetDegree(ISOLATED).Should().Be(0);
    }

    [Test]
    public void TheNeighborCountShouldMatchTheNeighbors()
    {
        _graph.GetNeighbors(HUB).Count.Should().Be(2);
    }
}
