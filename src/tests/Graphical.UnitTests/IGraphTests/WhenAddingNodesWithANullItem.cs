using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
[TestFixture(typeof(DirectedAcyclicGraph<>))]
[TestFixture(typeof(ReachabilityDirectedAcyclicGraph<>))]
internal sealed class WhenAddingNodesWithANullItem
{
    private readonly Type _graphType;

    private IGraph<string> _graph = null!;

    private Exception? _nullItemException;

    private Exception? _nullCollectionException;

    public WhenAddingNodesWithANullItem(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
        _nullItemException = Catch.Exception(() => _graph.AddNodes(["a", null!, "b"]));
        _nullCollectionException = Catch.Exception(() => _graph.AddNodes(null!));
    }

    [Test]
    public void ANullItemShouldThrowArgumentExceptionNamingTheNodes()
    {
        _nullItemException.Should().BeOfType<ArgumentException>().Which.ParamName.Should().Be("nodes");
    }

    [Test]
    public void ANullCollectionShouldThrowArgumentNullExceptionNamingTheNodes()
    {
        _nullCollectionException.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("nodes");
    }

    [Test]
    public void NoNodeShouldBeAdded()
    {
        _graph.Nodes.Should().BeEmpty();
    }
}
