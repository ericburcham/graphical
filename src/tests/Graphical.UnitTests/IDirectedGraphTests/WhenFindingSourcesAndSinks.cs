using AwesomeAssertions;

namespace Graphical.UnitTests.IDirectedGraphTests;

[TestFixture(typeof(DirectedGraph<>))]
[TestFixture(typeof(DirectedAcyclicGraph<>))]
[TestFixture(typeof(ReachabilityDirectedAcyclicGraph<>))]
internal sealed class WhenFindingSourcesAndSinks
{
    private readonly Type _graphType;

    private IReadOnlyCollection<string> _sources = null!;

    private IReadOnlyCollection<string> _sinks = null!;

    private int _sourcesCountBeforeChange;

    public WhenFindingSourcesAndSinks(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var graph = GraphFactory.CreateDirected<string>(_graphType);
        graph.AddEdges([Edge.Create("a", "b"), Edge.Create("b", "c"), Edge.Create("e", "c")]);
        graph.AddNode("d");

        _sources = graph.GetSources();
        _sinks = graph.GetSinks();
        _sourcesCountBeforeChange = _sources.Count;

        graph.AddEdge("b", "a2");
        graph.AddNode("f");
    }

    [Test]
    public void SourcesShouldBeTheNodesWithoutIncomingEdgesInInsertionOrder()
    {
        _sources.Should().Equal("a", "e", "d");
    }

    [Test]
    public void SinksShouldBeTheNodesWithoutOutgoingEdgesInInsertionOrder()
    {
        _sinks.Should().Equal("c", "d");
    }

    [Test]
    public void TheResultsShouldBeSnapshotsUnaffectedByLaterChanges()
    {
        _sources.Count.Should().Be(_sourcesCountBeforeChange);
    }
}
