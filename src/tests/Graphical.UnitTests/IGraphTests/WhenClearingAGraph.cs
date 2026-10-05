using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
[TestFixture(typeof(DirectedAcyclicGraph<>))]
[TestFixture(typeof(ReachabilityDirectedAcyclicGraph<>))]
internal sealed class WhenClearingAGraph
{
    private readonly Type _graphType;

    private IGraph<string> _graph = null!;

    private int _nodeCountAfterClear;

    private int _edgeCountAfterClear;

    private bool _containsOldNodeAfterClear;

    public WhenClearingAGraph(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
        _graph.AddEdges([Edge.Create("a", "b"), Edge.Create("b", "c")]);

        _graph.Clear();
        _nodeCountAfterClear = _graph.NodeCount;
        _edgeCountAfterClear = _graph.EdgeCount;
        _containsOldNodeAfterClear = _graph.ContainsNode("a");

        _graph.AddEdge("c", "d");
    }

    [Test]
    public void NodeCountShouldBeZero()
    {
        _nodeCountAfterClear.Should().Be(0);
    }

    [Test]
    public void EdgeCountShouldBeZero()
    {
        _edgeCountAfterClear.Should().Be(0);
    }

    [Test]
    public void OldNodesShouldBeGone()
    {
        _containsOldNodeAfterClear.Should().BeFalse();
    }

    [Test]
    public void TheGraphShouldBeUsableAgain()
    {
        _graph.Edges.Should().Equal(Edge.Create("c", "d"));
    }

    [Test]
    public void ReusedSlotsShouldNotKeepOldEdges()
    {
        _graph.GetDegree("c").Should().Be(1);
    }
}
