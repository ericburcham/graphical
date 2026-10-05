using AwesomeAssertions;

namespace Graphical.UnitTests.ReadmeTests;

// Mirrors the README "UndirectedGraph" sample.
[TestFixture]
internal sealed class WhenUsingAnUndirectedGraph
{
    private bool _containsReversedEdge;

    private bool _aliceReachesCarol;

    private bool _aliceReachesDave;

    private IReadOnlyCollection<string> _bobsNeighbors = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var network = new UndirectedGraph<string>();
        network.AddEdge("alice", "bob");
        network.AddEdge("bob", "carol");
        network.AddNode("dave");

        _containsReversedEdge = network.ContainsEdge("bob", "alice");   // true: edges have no direction
        _aliceReachesCarol = network.AreConnected("alice", "carol");    // true: same component
        _aliceReachesDave = network.AreConnected("alice", "dave");      // false
        _bobsNeighbors = network.GetNeighbors("bob");                   // alice, carol
    }

    [Test]
    public void ContainsEdgeShouldIgnoreDirection()
    {
        _containsReversedEdge.Should().BeTrue();
    }

    [Test]
    public void AliceShouldBeConnectedToCarol()
    {
        _aliceReachesCarol.Should().BeTrue();
    }

    [Test]
    public void AliceShouldNotBeConnectedToDave()
    {
        _aliceReachesDave.Should().BeFalse();
    }

    [Test]
    public void BobsNeighborsShouldBeAliceAndCarol()
    {
        _bobsNeighbors.Should().BeEquivalentTo(["alice", "carol"]);
    }
}
