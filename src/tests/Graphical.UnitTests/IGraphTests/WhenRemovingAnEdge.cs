using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
internal sealed class WhenRemovingAnEdge
{
    private const string SOURCE = "a";

    private const string TARGET = "b";

    private readonly Type _graphType;

    private IGraph<string> _graph = null!;

    private bool _removed;

    public WhenRemovingAnEdge(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
        _graph.AddEdge(SOURCE, TARGET);
        _removed = _graph.RemoveEdge(SOURCE, TARGET);
    }

    [Test]
    public void RemoveEdgeShouldReturnTrue()
    {
        _removed.Should().BeTrue();
    }

    [Test]
    public void TheGraphShouldNotContainTheEdge()
    {
        _graph.ContainsEdge(SOURCE, TARGET).Should().BeFalse();
    }

    [Test]
    public void EdgeCountShouldBeZero()
    {
        _graph.EdgeCount.Should().Be(0);
    }

    [Test]
    public void BothEndpointsShouldRemain()
    {
        _graph.Nodes.Should().Equal(SOURCE, TARGET);
    }

    [Test]
    public void TheSourceShouldHaveNoNeighbors()
    {
        _graph.GetNeighbors(SOURCE).Should().BeEmpty();
    }

    [Test]
    public void TheTargetShouldHaveNoNeighbors()
    {
        _graph.GetNeighbors(TARGET).Should().BeEmpty();
    }
}
