using AwesomeAssertions;

namespace Graphical.UnitTests.ReadmeTests;

// Mirrors the README "ReachabilityDirectedAcyclicGraph" sample.
[TestFixture]
internal sealed class WhenSwappingInTheReachabilityGraph
{
    private bool _hasPath;

    private bool _wouldCreateCycle;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var tasks = new ReachabilityDirectedAcyclicGraph<string>();   // was: new DirectedAcyclicGraph<string>()
        tasks.AddEdges(new[] { Edge.Create("design", "build"), Edge.Create("build", "test") });

        _hasPath = tasks.HasPath("design", "test");                     // true, in O(1)
        _wouldCreateCycle = tasks.WouldCreateCycle("test", "design");   // true, in O(1)
    }

    [Test]
    public void DesignShouldReachTest()
    {
        _hasPath.Should().BeTrue();
    }

    [Test]
    public void ClosingTheLoopShouldCreateACycle()
    {
        _wouldCreateCycle.Should().BeTrue();
    }
}
