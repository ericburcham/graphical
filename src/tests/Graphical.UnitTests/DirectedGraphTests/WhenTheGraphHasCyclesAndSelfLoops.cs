using AwesomeAssertions;

namespace Graphical.UnitTests.DirectedGraphTests;

[TestFixture]
internal sealed class WhenTheGraphHasCyclesAndSelfLoops
{
    private const string LOOP = "loop";

    private DirectedGraph<string> _graph = null!;

    private bool _addedSelfLoop;

    private int _edgeCountAfterRemovingTheLoopNode;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = new DirectedGraph<string>();
        _graph.AddEdges([Edge.Create("a", "b"), Edge.Create("b", "c"), Edge.Create("c", "a")]);
        _addedSelfLoop = _graph.AddEdge(LOOP, LOOP);
        _graph.AddEdge(LOOP, "a");

        var other = new DirectedGraph<string>();
        other.AddEdges([Edge.Create(LOOP, LOOP), Edge.Create(LOOP, "a"), Edge.Create("b", LOOP), Edge.Create("a", "b")]);
        other.RemoveNode(LOOP);
        _edgeCountAfterRemovingTheLoopNode = other.EdgeCount;
    }

    [Test]
    public void AddingASelfLoopShouldSucceed()
    {
        _addedSelfLoop.Should().BeTrue();
    }

    [Test]
    public void ANodeOnACycleShouldHaveAPathToItself()
    {
        _graph.HasPath("a", "a").Should().BeTrue();
    }

    [Test]
    public void ANodeWithASelfLoopShouldHaveAPathToItself()
    {
        _graph.HasPath(LOOP, LOOP).Should().BeTrue();
    }

    [Test]
    public void ASelfLoopShouldContributeTwoToTheDegree()
    {
        _graph.GetDegree(LOOP).Should().Be(3);
    }

    [Test]
    public void ASelfLoopShouldCountAsAnIncomingEdge()
    {
        _graph.GetInDegree(LOOP).Should().Be(1);
    }

    [Test]
    public void ASelfLoopNodeShouldNeighborItselfOnce()
    {
        _graph.GetNeighbors(LOOP).Should().BeEquivalentTo([LOOP, "a"]);
    }

    [Test]
    public void ACycleShouldLeaveNoSources()
    {
        _graph.GetSources().Should().BeEmpty();
    }

    [Test]
    public void RemovingTheLoopNodeShouldUncountEachIncidentEdgeOnce()
    {
        _edgeCountAfterRemovingTheLoopNode.Should().Be(1);
    }
}
