using AwesomeAssertions;

namespace Graphical.UnitTests.IDirectedGraphTests;

[TestFixture(typeof(DirectedGraph<>))]
[TestFixture(typeof(DirectedAcyclicGraph<>))]
internal sealed class WhenQueryingSuccessorsAndPredecessors
{
    private readonly Type _graphType;

    private IDirectedGraph<string> _graph = null!;

    private readonly List<string> _inconsistencies = [];

    public WhenQueryingSuccessorsAndPredecessors(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.CreateDirected<string>(_graphType);
        _graph.AddEdges([Edge.Create("a", "b"), Edge.Create("a", "c"), Edge.Create("b", "c"), Edge.Create("c", "d")]);

        foreach (var node in _graph.Nodes)
        {
            foreach (var successor in _graph.GetSuccessors(node))
            {
                if (!_graph.GetPredecessors(successor).Contains(node)) _inconsistencies.Add($"{node} -> {successor}");
            }
        }
    }

    [Test]
    public void SuccessorsShouldBeTheEdgeTargets()
    {
        _graph.GetSuccessors("a").Should().BeEquivalentTo(["b", "c"]);
    }

    [Test]
    public void PredecessorsShouldBeTheEdgeSources()
    {
        _graph.GetPredecessors("c").Should().BeEquivalentTo(["a", "b"]);
    }

    [Test]
    public void OutDegreeShouldCountSuccessors()
    {
        _graph.GetOutDegree("a").Should().Be(2);
    }

    [Test]
    public void InDegreeShouldCountPredecessors()
    {
        _graph.GetInDegree("c").Should().Be(2);
    }

    [Test]
    public void DegreeShouldBeInDegreePlusOutDegree()
    {
        _graph.GetDegree("c").Should().Be(3);
    }

    [Test]
    public void NeighborsShouldBeSuccessorsAndPredecessorsOnce()
    {
        _graph.GetNeighbors("c").Should().BeEquivalentTo(["a", "b", "d"]);
    }

    [Test]
    public void EveryEdgeShouldAppearFromBothEnds()
    {
        _inconsistencies.Should().BeEmpty();
    }

    [Test]
    public void IsDirectedShouldBeTrue()
    {
        _graph.IsDirected.Should().BeTrue();
    }
}
