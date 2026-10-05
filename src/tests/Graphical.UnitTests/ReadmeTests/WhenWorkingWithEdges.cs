using AwesomeAssertions;

namespace Graphical.UnitTests.ReadmeTests;

// Mirrors the README "Edges" sample.
[TestFixture]
internal sealed class WhenWorkingWithEdges
{
    private string _source = null!;

    private string _target = null!;

    private string _reversed = null!;

    private bool _equal;

    private bool _addedAgain;

    private int _nodeCount;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var edge = Edge.Create("a", "b");
        var (source, target) = edge;                    // "a", "b"
        _source = source;
        _target = target;
        _reversed = edge.Reverse().ToString();          // "(b -> a)"
        _equal = edge == new Edge<string>("a", "b");    // true

        var graph = new DirectedGraph<string>();
        graph.AddEdge("a", "b");                        // adds "a" and "b" too
        _addedAgain = graph.AddEdge("a", "b");          // false: graphs are simple
        _nodeCount = graph.NodeCount;                   // 2
    }

    [Test]
    public void DeconstructingShouldGiveTheSource()
    {
        _source.Should().Be("a");
    }

    [Test]
    public void DeconstructingShouldGiveTheTarget()
    {
        _target.Should().Be("b");
    }

    [Test]
    public void ReversingShouldSwapTheEndpoints()
    {
        _reversed.Should().Be("(b -> a)");
    }

    [Test]
    public void EdgesWithTheSameEndpointsShouldBeEqual()
    {
        _equal.Should().BeTrue();
    }

    [Test]
    public void AddingAnExistingEdgeShouldReturnFalse()
    {
        _addedAgain.Should().BeFalse();
    }

    [Test]
    public void AddingAnEdgeShouldAddItsEndpoints()
    {
        _nodeCount.Should().Be(2);
    }
}
