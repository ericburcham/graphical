using AwesomeAssertions;

namespace Graphical.UnitTests.UndirectedGraphTests;

[TestFixture]
internal sealed class WhenRemovingAnEdgeByItsReverse
{
    private UndirectedGraph<string> _graph = null!;

    private bool _removed;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = new UndirectedGraph<string>();
        _graph.AddEdge("a", "b");
        _removed = _graph.RemoveEdge("b", "a");
    }

    [Test]
    public void RemoveEdgeShouldReturnTrue()
    {
        _removed.Should().BeTrue();
    }

    [Test]
    public void TheEdgeShouldBeGoneInBothDirections()
    {
        _graph.ContainsEdge("a", "b").Should().BeFalse();
    }

    [Test]
    public void EdgeCountShouldBeZero()
    {
        _graph.EdgeCount.Should().Be(0);
    }
}
