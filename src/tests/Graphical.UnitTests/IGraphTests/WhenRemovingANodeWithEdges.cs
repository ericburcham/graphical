using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
[TestFixture(typeof(DirectedAcyclicGraph<>))]
internal sealed class WhenRemovingANodeWithEdges
{
    private const string REMOVED = "hub";

    private const string FIRST = "first";

    private const string SECOND = "second";

    private readonly Type _graphType;

    private IGraph<string> _graph = null!;

    private bool _removed;

    public WhenRemovingANodeWithEdges(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
        _graph.AddEdge(REMOVED, FIRST);
        _graph.AddEdge(REMOVED, SECOND);
        _graph.AddEdge(FIRST, SECOND);

        _removed = _graph.RemoveNode(REMOVED);
    }

    [Test]
    public void RemoveNodeShouldReturnTrue()
    {
        _removed.Should().BeTrue();
    }

    [Test]
    public void TheGraphShouldNotContainTheNode()
    {
        _graph.ContainsNode(REMOVED).Should().BeFalse();
    }

    [Test]
    public void NodesShouldHoldOnlyTheOtherNodes()
    {
        _graph.Nodes.Should().Equal(FIRST, SECOND);
    }

    [Test]
    public void IncidentEdgesShouldBeRemoved()
    {
        _graph.Edges.Should().Equal(Edge.Create(FIRST, SECOND));
    }

    [Test]
    public void EdgeCountShouldCountOnlyTheRemainingEdge()
    {
        _graph.EdgeCount.Should().Be(1);
    }

    [Test]
    public void FormerNeighborsShouldNoLongerListTheNode()
    {
        _graph.GetNeighbors(FIRST).Should().Equal(SECOND);
    }
}
