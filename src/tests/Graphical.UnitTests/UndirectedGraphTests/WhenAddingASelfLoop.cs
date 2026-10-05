using AwesomeAssertions;

namespace Graphical.UnitTests.UndirectedGraphTests;

[TestFixture]
internal sealed class WhenAddingASelfLoop
{
    private const string NODE = "a";

    private const string OTHER = "b";

    private UndirectedGraph<string> _graph = null!;

    private bool _added;

    private int _edgeCountAfterRemovingTheNode;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = new UndirectedGraph<string>();
        _added = _graph.AddEdge(NODE, NODE);

        var other = new UndirectedGraph<string>();
        other.AddEdge(NODE, NODE);
        other.AddEdge(NODE, OTHER);
        other.RemoveNode(NODE);
        _edgeCountAfterRemovingTheNode = other.EdgeCount;
    }

    [Test]
    public void AddEdgeShouldReturnTrue()
    {
        _added.Should().BeTrue();
    }

    [Test]
    public void EdgeCountShouldCountTheLoopOnce()
    {
        _graph.EdgeCount.Should().Be(1);
    }

    [Test]
    public void TheLoopShouldContributeTwoToTheDegree()
    {
        _graph.GetDegree(NODE).Should().Be(2);
    }

    [Test]
    public void TheNodeShouldNeighborItselfOnce()
    {
        _graph.GetNeighbors(NODE).Should().Equal(NODE);
    }

    [Test]
    public void EdgesShouldListTheLoopOnce()
    {
        _graph.Edges.Should().Equal(Edge.Create(NODE, NODE));
    }

    [Test]
    public void RemovingTheNodeShouldUncountTheLoopAndItsOtherEdges()
    {
        _edgeCountAfterRemovingTheNode.Should().Be(0);
    }
}
