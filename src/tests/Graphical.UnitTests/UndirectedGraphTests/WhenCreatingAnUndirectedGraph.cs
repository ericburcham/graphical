using AwesomeAssertions;

namespace Graphical.UnitTests.UndirectedGraphTests;

[TestFixture]
internal sealed class WhenCreatingAnUndirectedGraph
{
    private UndirectedGraph<int> _graph = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = new UndirectedGraph<int>();
    }

    [Test]
    public void IsDirectedShouldBeFalse()
    {
        _graph.IsDirected.Should().BeFalse();
    }
}
