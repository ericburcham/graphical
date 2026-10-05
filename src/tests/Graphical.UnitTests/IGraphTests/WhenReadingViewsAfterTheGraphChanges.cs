using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
internal sealed class WhenReadingViewsAfterTheGraphChanges
{
    private readonly Type _graphType;

    private IReadOnlyCollection<string> _nodes = null!;

    private IReadOnlyCollection<Edge<string>> _edges = null!;

    private IReadOnlyCollection<string> _neighbors = null!;

    public WhenReadingViewsAfterTheGraphChanges(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var graph = GraphFactory.Create<string>(_graphType);
        graph.AddNode("a");
        _nodes = graph.Nodes;
        _edges = graph.Edges;
        _neighbors = graph.GetNeighbors("a");

        graph.AddEdge("a", "b");
    }

    [Test]
    public void TheNodesViewShouldShowTheNewNode()
    {
        _nodes.Should().Equal("a", "b");
    }

    [Test]
    public void TheEdgesViewShouldShowTheNewEdge()
    {
        _edges.Should().Equal(Edge.Create("a", "b"));
    }

    [Test]
    public void TheNeighborsViewShouldShowTheNewNeighbor()
    {
        _neighbors.Should().Equal("b");
    }
}
