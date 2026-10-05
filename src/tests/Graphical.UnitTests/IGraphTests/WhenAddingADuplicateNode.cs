using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
[TestFixture(typeof(DirectedAcyclicGraph<>))]
internal sealed class WhenAddingADuplicateNode
{
    private const string NODE = "a";

    private readonly Type _graphType;

    private IGraph<string> _graph = null!;

    private bool _added;

    public WhenAddingADuplicateNode(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
        _graph.AddNode(NODE);
        _added = _graph.AddNode(NODE);
    }

    [Test]
    public void AddNodeShouldReturnFalse()
    {
        _added.Should().BeFalse();
    }

    [Test]
    public void NodeCountShouldStayOne()
    {
        _graph.NodeCount.Should().Be(1);
    }
}
