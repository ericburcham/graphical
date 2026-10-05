using AwesomeAssertions;

namespace Graphical.UnitTests.UndirectedGraphTests;

[TestFixture]
internal sealed class WhenAddingAnEdgeAgainstInsertionOrder
{
    private const string EARLIER = "earlier";

    private const string LATER = "later";

    private UndirectedGraph<string> _graph = null!;

    private bool _addedReverse;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = new UndirectedGraph<string>();
        _graph.AddNode(EARLIER);
        _graph.AddNode(LATER);
        _graph.AddEdge(LATER, EARLIER);
        _addedReverse = _graph.AddEdge(EARLIER, LATER);
    }

    [Test]
    public void ContainsEdgeShouldBeTrueAsAdded()
    {
        _graph.ContainsEdge(LATER, EARLIER).Should().BeTrue();
    }

    [Test]
    public void ContainsEdgeShouldBeTrueReversed()
    {
        _graph.ContainsEdge(EARLIER, LATER).Should().BeTrue();
    }

    [Test]
    public void AddingTheReverseShouldBeADuplicate()
    {
        _addedReverse.Should().BeFalse();
    }

    [Test]
    public void EdgesShouldListTheEdgeOnceFromTheEarlierNode()
    {
        _graph.Edges.Should().Equal(Edge.Create(EARLIER, LATER));
    }

    [Test]
    public void EdgeCountShouldBeOne()
    {
        _graph.EdgeCount.Should().Be(1);
    }

    [Test]
    public void TheLaterNodeShouldNeighborTheEarlierNode()
    {
        _graph.GetNeighbors(LATER).Should().Equal(EARLIER);
    }

    [Test]
    public void TheEarlierNodeShouldNeighborTheLaterNode()
    {
        _graph.GetNeighbors(EARLIER).Should().Equal(LATER);
    }
}
