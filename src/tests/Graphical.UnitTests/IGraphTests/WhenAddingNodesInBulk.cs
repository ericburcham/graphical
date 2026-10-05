using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
internal sealed class WhenAddingNodesInBulk
{
    private const string EXISTING = "b";

    private readonly Type _graphType;

    private IGraph<string> _graph = null!;

    private int _added;

    public WhenAddingNodesInBulk(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
        _graph.AddNode(EXISTING);
        _added = _graph.AddNodes(["a", EXISTING, "a", "c"]);
    }

    [Test]
    public void AddNodesShouldReturnTheNumberActuallyAdded()
    {
        _added.Should().Be(2);
    }

    [Test]
    public void NodesShouldHoldEachNodeOnceInInsertionOrder()
    {
        _graph.Nodes.Should().Equal(EXISTING, "a", "c");
    }
}
