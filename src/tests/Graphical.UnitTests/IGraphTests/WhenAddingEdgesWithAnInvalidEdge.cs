using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
internal sealed class WhenAddingEdgesWithAnInvalidEdge
{
    private readonly Type _graphType;

    private IGraph<string> _graph = null!;

    private Exception? _defaultEdgeException;

    private Exception? _nullTargetException;

    private Exception? _nullCollectionException;

    public WhenAddingEdgesWithAnInvalidEdge(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
        _defaultEdgeException = Catch.Exception(() => _graph.AddEdges([Edge.Create("a", "b"), default]));
        _nullTargetException = Catch.Exception(() => _graph.AddEdges([Edge.Create("a", "b"), Edge.Create("c", (string)null!)]));
        _nullCollectionException = Catch.Exception(() => _graph.AddEdges(null!));
    }

    [Test]
    public void ADefaultEdgeShouldThrowArgumentExceptionNamingTheEdges()
    {
        _defaultEdgeException.Should().BeOfType<ArgumentException>().Which.ParamName.Should().Be("edges");
    }

    [Test]
    public void ANullTargetShouldThrowArgumentExceptionNamingTheEdges()
    {
        _nullTargetException.Should().BeOfType<ArgumentException>().Which.ParamName.Should().Be("edges");
    }

    [Test]
    public void ANullCollectionShouldThrowArgumentNullExceptionNamingTheEdges()
    {
        _nullCollectionException.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("edges");
    }

    [Test]
    public void NoNodeShouldBeAdded()
    {
        _graph.Nodes.Should().BeEmpty();
    }

    [Test]
    public void NoEdgeShouldBeAdded()
    {
        _graph.EdgeCount.Should().Be(0);
    }
}
