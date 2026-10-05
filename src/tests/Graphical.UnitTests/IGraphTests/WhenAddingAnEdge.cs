using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
internal sealed class WhenAddingAnEdge
{
    private const string SOURCE = "a";

    private const string TARGET = "b";

    private readonly Type _graphType;

    private IGraph<string> _graph = null!;

    private bool _added;

    public WhenAddingAnEdge(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
        _added = _graph.AddEdge(SOURCE, TARGET);
    }

    [Test]
    public void AddEdgeShouldReturnTrue()
    {
        _added.Should().BeTrue();
    }

    [Test]
    public void TheGraphShouldContainTheEdge()
    {
        _graph.ContainsEdge(SOURCE, TARGET).Should().BeTrue();
    }

    [Test]
    public void EdgeCountShouldBeOne()
    {
        _graph.EdgeCount.Should().Be(1);
    }

    [Test]
    public void EdgesShouldHoldTheEdge()
    {
        _graph.Edges.Should().Equal(Edge.Create(SOURCE, TARGET));
    }

    [Test]
    public void BothEndpointsShouldBeAdded()
    {
        _graph.Nodes.Should().Equal(SOURCE, TARGET);
    }
}
