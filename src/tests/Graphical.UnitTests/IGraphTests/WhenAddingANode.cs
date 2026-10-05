using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
internal sealed class WhenAddingANode
{
    private const string NODE = "a";

    private readonly Type _graphType;

    private IGraph<string> _graph = null!;

    private bool _added;

    public WhenAddingANode(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
        _added = _graph.AddNode(NODE);
    }

    [Test]
    public void AddNodeShouldReturnTrue()
    {
        _added.Should().BeTrue();
    }

    [Test]
    public void TheGraphShouldContainTheNode()
    {
        _graph.ContainsNode(NODE).Should().BeTrue();
    }

    [Test]
    public void NodeCountShouldBeOne()
    {
        _graph.NodeCount.Should().Be(1);
    }

    [Test]
    public void NodesShouldHoldTheNode()
    {
        _graph.Nodes.Should().Equal(NODE);
    }

    [Test]
    public void EdgeCountShouldStayZero()
    {
        _graph.EdgeCount.Should().Be(0);
    }
}
