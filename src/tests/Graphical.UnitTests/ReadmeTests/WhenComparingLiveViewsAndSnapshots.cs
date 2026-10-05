using AwesomeAssertions;

namespace Graphical.UnitTests.ReadmeTests;

// Mirrors the README "Live views vs. snapshots" sample.
[TestFixture]
internal sealed class WhenComparingLiveViewsAndSnapshots
{
    private int _nodesCount;

    private int _sourcesCount;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var graph = new DirectedGraph<int>();
        var nodes = graph.Nodes;            // live view
        var sources = graph.GetSources();   // snapshot

        graph.AddNode(1);

        _nodesCount = nodes.Count;          // 1: the view sees the new node
        _sourcesCount = sources.Count;      // 0: the snapshot does not
    }

    [Test]
    public void TheLiveViewShouldSeeTheNewNode()
    {
        _nodesCount.Should().Be(1);
    }

    [Test]
    public void TheSnapshotShouldNotSeeTheNewNode()
    {
        _sourcesCount.Should().Be(0);
    }
}
