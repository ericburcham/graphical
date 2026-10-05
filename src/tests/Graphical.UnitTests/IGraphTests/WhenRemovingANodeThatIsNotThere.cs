using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
[TestFixture(typeof(DirectedAcyclicGraph<>))]
internal sealed class WhenRemovingANodeThatIsNotThere
{
    private readonly Type _graphType;

    private IGraph<string> _graph = null!;

    private bool _removed;

    public WhenRemovingANodeThatIsNotThere(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
        _graph.AddNode("a");
        _removed = _graph.RemoveNode("z");
    }

    [Test]
    public void RemoveNodeShouldReturnFalse()
    {
        _removed.Should().BeFalse();
    }

    [Test]
    public void NodeCountShouldBeUnchanged()
    {
        _graph.NodeCount.Should().Be(1);
    }
}
