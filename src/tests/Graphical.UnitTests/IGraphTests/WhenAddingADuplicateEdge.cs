using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
[TestFixture(typeof(DirectedAcyclicGraph<>))]
internal sealed class WhenAddingADuplicateEdge
{
    private const string SOURCE = "a";

    private const string TARGET = "b";

    private readonly Type _graphType;

    private IGraph<string> _graph = null!;

    private bool _added;

    public WhenAddingADuplicateEdge(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
        _graph.AddEdge(SOURCE, TARGET);
        _added = _graph.AddEdge(SOURCE, TARGET);
    }

    [Test]
    public void AddEdgeShouldReturnFalse()
    {
        _added.Should().BeFalse();
    }

    [Test]
    public void EdgeCountShouldStayOne()
    {
        _graph.EdgeCount.Should().Be(1);
    }
}
