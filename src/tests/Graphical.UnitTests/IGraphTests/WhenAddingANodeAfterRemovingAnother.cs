using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
internal sealed class WhenAddingANodeAfterRemovingAnother
{
    private const string KEPT = "kept";

    private const string REMOVED = "removed";

    private const string REPLACEMENT = "replacement";

    private readonly Type _graphType;

    private IGraph<string> _graph = null!;

    private IReadOnlyCollection<string> _removedNeighborsView = null!;

    public WhenAddingANodeAfterRemovingAnother(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
        _graph.AddEdge(KEPT, REMOVED);
        _removedNeighborsView = _graph.GetNeighbors(REMOVED);
        _graph.RemoveNode(REMOVED);
        _graph.AddNode(REPLACEMENT);
        _graph.AddEdge(REPLACEMENT, KEPT);
        _graph.RemoveEdge(REPLACEMENT, KEPT);
    }

    [Test]
    public void TheNewNodeShouldNotInheritTheOldEdges()
    {
        _graph.GetDegree(REPLACEMENT).Should().Be(0);
    }

    [Test]
    public void TheKeptNodeShouldHaveNoNeighbors()
    {
        _graph.GetNeighbors(KEPT).Should().BeEmpty();
    }

    [Test]
    public void NodesShouldListTheReplacementInTheReusedSlot()
    {
        _graph.Nodes.Should().Equal(KEPT, REPLACEMENT);
    }

    [Test]
    public void AViewOfTheRemovedNodeShouldStayEmpty()
    {
        _removedNeighborsView.Should().BeEmpty();
    }
}
