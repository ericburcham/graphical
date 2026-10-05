using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
[TestFixture(typeof(DirectedAcyclicGraph<>))]
internal sealed class WhenCreatingAGraph
{
    private readonly Type _graphType;

    private IGraph<string> _graph = null!;

    public WhenCreatingAGraph(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
    }

    [Test]
    public void NodeCountShouldBeZero()
    {
        _graph.NodeCount.Should().Be(0);
    }

    [Test]
    public void EdgeCountShouldBeZero()
    {
        _graph.EdgeCount.Should().Be(0);
    }

    [Test]
    public void NodesShouldBeEmpty()
    {
        _graph.Nodes.Should().BeEmpty();
    }

    [Test]
    public void EdgesShouldBeEmpty()
    {
        _graph.Edges.Should().BeEmpty();
    }

    [Test]
    public void TheComparerShouldBeTheDefault()
    {
        _graph.Comparer.Should().BeSameAs(EqualityComparer<string>.Default);
    }
}
