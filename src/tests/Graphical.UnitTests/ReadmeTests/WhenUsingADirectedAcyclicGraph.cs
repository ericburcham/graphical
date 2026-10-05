using AwesomeAssertions;

namespace Graphical.UnitTests.ReadmeTests;

// Mirrors the README "DirectedAcyclicGraph" sample.
[TestFixture]
internal sealed class WhenUsingADirectedAcyclicGraph
{
    private bool _wouldCreateCycle;

    private IReadOnlyCollection<string> _ancestors = null!;

    private IReadOnlyCollection<string> _descendants = null!;

    private bool _threw;

    private bool _containsRelease;

    private IReadOnlyList<string> _order = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var tasks = new DirectedAcyclicGraph<string>();
        tasks.AddEdges(new[] { Edge.Create("design", "build"), Edge.Create("build", "test"), Edge.Create("build", "docs") });

        _wouldCreateCycle = tasks.WouldCreateCycle("test", "design");   // true
        _ancestors = tasks.GetAncestors("test");                        // design, build
        _descendants = tasks.GetDescendants("design");                  // build, test, docs

        try
        {
            // Together these two edges close a cycle, so neither is added.
            tasks.AddEdges(new[] { Edge.Create("test", "release"), Edge.Create("release", "build") });
        }
        catch (GraphCycleException)
        {
            _threw = true;
        }

        _containsRelease = tasks.ContainsNode("release");               // false: the batch is atomic
        _order = tasks.GetTopologicalOrder();                           // design, build, test, docs
    }

    [Test]
    public void ClosingTheLoopShouldCreateACycle()
    {
        _wouldCreateCycle.Should().BeTrue();
    }

    [Test]
    public void TestShouldHaveDesignAndBuildAsAncestors()
    {
        _ancestors.Should().Equal("design", "build");
    }

    [Test]
    public void DesignShouldHaveEveryOtherTaskAsADescendant()
    {
        _descendants.Should().Equal("build", "test", "docs");
    }

    [Test]
    public void TheCycleCreatingBatchShouldThrow()
    {
        _threw.Should().BeTrue();
    }

    [Test]
    public void TheRejectedBatchShouldAddNothing()
    {
        _containsRelease.Should().BeFalse();
    }

    [Test]
    public void TheOrderShouldPutDependenciesFirst()
    {
        _order.Should().Equal("design", "build", "test", "docs");
    }
}
